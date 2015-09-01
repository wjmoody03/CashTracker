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


    }
}