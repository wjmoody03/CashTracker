using System.IO;
using ct.Business;
using ct.Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ct.Domain.Models;

namespace ct.Web.Controllers
{
    public class UtilityController : Controller
    {
        IAccountRepository acctRepo;
        ITransactionRepository transRepo;

        public UtilityController(IAccountRepository AccountRepo, ITransactionRepository TransactionRepo)
        {
            acctRepo = AccountRepo;
            transRepo = TransactionRepo;
        }
        // GET: Utility
        [HttpPost]
        [Authorize]
        public string DownloadNewTransactions(int AccountID)
        {
            var acct = acctRepo.FindBy(a => a.AccountID == AccountID).FirstOrDefault();
            var ccd = new CreditCardTransactionDownloader(acct, transRepo);
            var adr = ccd.GetAllTransactions();
            acct.LastImport = DateTime.Now;
            acct.StatedBalanceAtInstitution = adr.AccountBalance;
            acctRepo.Edit(acct);
            acctRepo.Save();
            transRepo.AddRange(adr.NewTransactions);
            transRepo.Save();
            return JsonConvert.SerializeObject(adr);
        }


        //USED FOR RETROACTIVELY APPLYING UNIQUE BANK IDs TO TRANSACTIONS
        //int updated = 0;
        //for (int ix = adr.NewTransactions.Count - 1; ix >= 0; ix--)
        //{
        //    if (adr.NewTransactions[ix].FlagForFollowUp)
        //    {
        //        var id = int.Parse(adr.NewTransactions[ix].Notes.Replace("This is a possible duplicate of existing transaction ID ", "").Split(' ')[0]);
        //        var ex = transRepo.FindBy(t => t.ID == id).First();
        //        ex.SourceTransactionIdentifier = adr.NewTransactions[ix].SourceTransactionIdentifier;
        //        ex.Notes += "Source ID mapped during migration.";
        //        transRepo.Edit(ex);
        //        updated++;
        //    }
        //}
        //transRepo.Save();
        //return updated.ToString();

    }
}