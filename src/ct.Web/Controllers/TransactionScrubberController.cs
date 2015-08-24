using ct.Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ct.Web.Controllers
{
    public class TransactionScrubberController : Controller
    {
        ITransactionRepository transactionRepo;

        public TransactionScrubberController(ITransactionRepository TransactionRepo)
        {
            transactionRepo = TransactionRepo;
        }

        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public void SetCategory(string Category, int TransactionID)
        {
            var trans = transactionRepo.FindBy(t => t.ID == TransactionID).FirstOrDefault();
            if(trans!= null)
            {
                trans.Category = Category;
                transactionRepo.Edit(trans);
                transactionRepo.Save();
            }
            else
            {
                throw new Exception("Transaction not found");
            }
        }

        // GET: TransactionScrubber
        public string TransactionsNeedingAttention()
        {
            var transactions = transactionRepo.TransactionsNeedingAttention();
            return JsonConvert.SerializeObject(transactions);
        }

        public string ProbableCategories()
        {
            var cat = transactionRepo.ProbableCategories();
            return JsonConvert.SerializeObject(cat);
        }
    }
}