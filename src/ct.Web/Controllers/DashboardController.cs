using ct.Data.Repositories;
using ct.Domain.Models;
using ct.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ct.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private IAccountBalanceRepository balanceRepo { get; set; }
        private ITransactionRepository transRepo { get; set; } 

        public DashboardController(IAccountBalanceRepository BalanceRepo, ITransactionRepository TransactionRepo)
        {
            balanceRepo = BalanceRepo;
            transRepo = TransactionRepo;
        }

        public string SnapshotHistory(DateTime? StartDate = null, DateTime? EndDate = null) 
        {
            if (StartDate == null) StartDate = DateTime.Today;
            if (EndDate == null) EndDate = DateTime.Today;

            var hist = balanceRepo.BalanceSnapshot((DateTime)StartDate, (DateTime)EndDate);
            return JsonConvert.SerializeObject(hist);
        }

        public string FlaggedTransactions()
        {
            var trans = transRepo.GetAllEagerly("TransactionType","Account").Where(t => t.FlagForFollowUp);
            var transModel = AutoMapper.Mapper.Map<IEnumerable<Transaction>,IEnumerable<TransactionViewModel>> (trans);
            return JsonConvert.SerializeObject(transModel);
        }

        public string ReimbursableBalances()
        {
            var trans = transRepo.GetAllEagerly("TransactionType","Account").Where(t => t.ReimbursableSource != null && t.ReimbursableSource.Trim() != "");
            var bal = from t in trans
                      group t by t.ReimbursableSource into tr
                      where tr.Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier) !=0
                      select new
                      {
                          ReimbursableSource = tr.Key,
                          ReimbursableBalance = tr.Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier),
                          StartDate = tr.Min(t=>t.TransactionDate),
                          Categories = from trc in tr
                                       group trc by trc.Category into trct
                                       where trct.Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier)!=0
                                       select new
                                       {
                                           Category = trct.Key,
                                           CategoryBalance = trct.Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier),
                                           StartDate = trct.Min(t => t.TransactionDate)
                                           //Transactions = trct eventually figure out how to only show transactions since the balance was last 0
                                       }
                      };
            return JsonConvert.SerializeObject(bal);
        }
    }
}