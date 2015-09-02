using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using ct.Business;
using ct.Data.Repositories;
using Newtonsoft.Json;
using ct.Data.Contexts;
using ct.Domain;

namespace ct.AutoDownloader
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private static IctContext cxt;
        private static IAccountRepository acctRepo;
        private static ITransactionRepository transRepo;

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            cxt = new ctContext(CashTrackerConfigurationManager.AzureSQLConnectionString);
            acctRepo = new AccountRepository(cxt);
            transRepo = new TransactionRepository(cxt);
            var acct = acctRepo.FindBy(a => a.AccountID == 24).FirstOrDefault(); //chase account
            var ccd = new CreditCardTransactionDownloader(acct, transRepo);
            var adr = ccd.GetAllTransactions();
            acct.LastImport = DateTime.Now;
            acct.StatedBalanceAtInstitution = adr.AccountBalance;
            acctRepo.Edit(acct);
            acctRepo.Save();
            transRepo.AddRange(adr.NewTransactions);
            transRepo.Save();
        }
    }
}
