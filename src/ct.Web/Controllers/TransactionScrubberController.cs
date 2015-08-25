using ct.Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ct.Web.Controllers
{
    [Authorize]
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
        public void SetCategory(string Category, int TransactionID, string Notes)
        {
            var trans = transactionRepo.FindBy(t => t.ID == TransactionID).FirstOrDefault();
            if (trans != null)
            {
                trans.Category = Category;
                trans.Notes = Notes;
                transactionRepo.Edit(trans);
                transactionRepo.Save();
            }
            else
            {
                throw new Exception("Transaction not found");
            }
        }
        [HttpPost]
        public void FlagForFollowUp(int TransactionID, string Notes)
        {
            var trans = transactionRepo.FindBy(t => t.ID == TransactionID).FirstOrDefault();
            if (trans != null)
            {
                trans.Notes = Notes;
                trans.FlagForFollowUp = true;
                transactionRepo.Edit(trans);
                transactionRepo.Save();
            }
            else
            {
                throw new Exception("Transaction not found");
            }
        }

        [HttpPost]
        public void RemoveFlag(int TransactionID)
        {
            var trans = transactionRepo.FindBy(t => t.ID == TransactionID).FirstOrDefault();
            if (trans != null)
            {
                trans.FlagForFollowUp = false;
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