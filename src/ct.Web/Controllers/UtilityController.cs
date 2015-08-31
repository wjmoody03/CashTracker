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
        //public void DownloadNewTransactions(int AccountID) 
        //{
        //    var acct = acctRepo.FindBy(a => a.AccountID == AccountID).FirstOrDefault();
        //    var ccd = new CreditCardTransactionDownloader(acct, transRepo);
        //    var trans = ccd.GetAllTransactions().OrderBy(t => t.TransactionDate);
        //    var json = JsonConvert.SerializeObject(trans);
        //    var fs = System.IO.File.CreateText("C:\\users\\jacob\\desktop\\finally.json");
        //    fs.Write(json);
        //    fs.Close();
        //    fs.Dispose();
        //}


    }
}