//using ct.Data.Repositories;
//using ct.Domain.Models;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.WindowsAzure.Storage.Table;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ct.Tests.Data.Repositories
//{
//    [TestClass]
//    public class TransactionRepositoryTests
//    {
//        [TestMethod]
//        public void transactions_table_is_created()
//        {
//            var x = Task.Run(async () =>
//            {
//                var repo = new TransactionRepository();
//                var t = new Transaction();
//                t.UserID = "jmoody";
//                t.Description = "test";
//                t.TransactionDate = DateTime.Today;
//                t.AccountID = 3;
//                t.Amount = 100;
//                t.typo = ttype.exp;
//                t.SplitTransactions = new List<Transaction>() {
//                    new Transaction()
//                    {
//                        UserID = "jmoody",
//                        TransactionDate = DateTime.Today,
//                        Description = "hey!"
//                    }
//                };

//                var tr = await repo.Add(t);
//                return tr;
//            }).GetAwaiter().GetResult();
//        }

//        [TestMethod]
//        public void transaction_retrieved() {
//            var repo = new TransactionRepository();
//            var t = repo.Get("jmoody", "c5dc2514-d6f9-4200-a609-8c71b0037dcf");
//            Assert.IsNotNull(t);
//        }

//    }
//}
