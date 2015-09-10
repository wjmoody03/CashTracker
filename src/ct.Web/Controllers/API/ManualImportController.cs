using ct.Business;
using ct.Business.OFX.Parsers;
using ct.Data.Repositories;
using ct.Domain.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ct.Web.Controllers.API
{
    public class ManualImportController : ApiController
    {

        //following sample from:
        //http://www.mono-software.com/blog/post/Mono/233/Async-upload-using-angular-file-upload-directive-and-net-WebAPI-service/
        IAccountRepository acctRepo;
        ITransactionRepository transRepo;
        IAccountDownloadResultRepository downloadRepo;

        public ManualImportController(IAccountRepository AccountRepo, ITransactionRepository TransactionRepo, IAccountDownloadResultRepository DownloadRepo)
        {
            acctRepo = AccountRepo;
            transRepo = TransactionRepo;
            downloadRepo = DownloadRepo;
        }

        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            //get the file data from the request and save it out to the working location
            var provider = GetMultipartProvider();
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            var originalFileName = GetDeserializedFileName(result.FileData.First());

            var ofx = File.ReadAllText(result.FileData.First().LocalFileName);
            var parser = new OFXParser(ofx);
            var acctID = parser.GetAccountID();
            var acct = acctRepo.GetAll().ToList().Where(a => string.IsNullOrWhiteSpace(a.EncryptedAccountNumber)?false: Encryptor.Decrypt(a.EncryptedAccountNumber)==acctID).FirstOrDefault();
            if (acct == null)
                throw new Exception("Account number cannot be found in OFX or account number does not exist in your setup.");

            var acctType = (AccountType)Enum.Parse(typeof(AccountType), acct.AccountType, true);
            var adr = new AccountDownloadResult();
            adr.AccountID = acct.AccountID;
            adr.StartTime = DateTime.Now;

            var downloadedTransactions = parser.GetTransactions();
            adr.TotalTransactionsDownloaded = downloadedTransactions.Count();
            adr.AccountBalance = parser.GetOutstandingBalance(acctType);

            adr.NewTransactions = (from p in downloadedTransactions
                                   select new Transaction()
                                   {
                                       AccountID = acct.AccountID,
                                       Amount = p.TRNAMT,
                                       Description = p.NAME,
                                       SourceTransactionIdentifier = p.FITID,
                                       TransactionDate = p.DTPOSTED,
                                       Notes = p.MEMO,
                                       TransactionTypeID = TransactionDownloader.TransactionTypeIDFromTypeAndDescription(p.TRNTYPE, p.NAME, acctType)
                                   }).ToList();

            var earliestTransactionDownloaded = adr.NewTransactions.Min(t => t.TransactionDate);
            var allTrans = transRepo.GetAll().Where(t => t.TransactionDate >= earliestTransactionDownloaded);
            TransactionUniquenessDetector.RemoveExistingTransactionsAndApplyFlagsToPossibleDupes(allTrans, ref adr);
            CategoryGuesser.ApplyCategories(transRepo.CategoryGuesses(), ref adr);
            adr.NetNewTransactions = adr.NewTransactions.Count;
            adr.EndTime = DateTime.Now;

            acct.LastImport = DateTime.Now;
            acct.StatedBalanceAtInstitution = adr.AccountBalance;
            acctRepo.Edit(acct);
            acctRepo.Save();
            transRepo.AddRange(adr.NewTransactions);
            transRepo.Save();
            downloadRepo.Add(adr);
            downloadRepo.Save();

            return this.Request.CreateResponse(HttpStatusCode.OK, new { adr });
        }

        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            // IMPORTANT: replace "(tilde)" with the real tilde character
            // (our editor doesn't allow it, so I just wrote "(tilde)" instead)
            var uploadFolder = Path.GetTempPath();  //"(tilde)/App_Data/Tmp/FileUploads"; // you could put this to web.config
            //var root = HttpContext.Current.Server.MapPath(uploadFolder);
            //Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(uploadFolder); //root);
        }

        // Extracts Request FormatData as a strongly typed model
        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData
                    .GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
            }

            return null;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

    }
}