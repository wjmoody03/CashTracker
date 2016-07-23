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
        private IBudgetRepository budgetRepo { get; set; }

        public DashboardController(IAccountBalanceRepository BalanceRepo, ITransactionRepository TransactionRepo, IBudgetRepository BudgetRepo)
        {
            balanceRepo = BalanceRepo;
            transRepo = TransactionRepo;
            budgetRepo = BudgetRepo;
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

        public string InvalidSplits()
        {
            var s = balanceRepo.InvalidSplits();
            return JsonConvert.SerializeObject(s);
        }

        public string ReimbursableBalances()
        {

            var splits = transRepo.GetAll().Select(t => t.ParentTransactionID ?? 0).Distinct().ToDictionary(t=> t) ;
            var trans = transRepo.GetAllEagerly("TransactionType", "Account")
                    .Where(t => t.ReimbursableSource != null && t.ReimbursableSource.Trim() != "")
                    .ToList()
                    .Where(t => !splits.ContainsKey(t.ID)); //exclude splits


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

        public string CurrentCategoryDistribution()
        {
            var thisMonth = DateTime.Today.Month;
            var thisMonthYear = DateTime.Today.Year;
            var results = transRepo.CategoryExpensesVsBudget(thisMonth, thisMonthYear);
            return JsonConvert.SerializeObject(results);
        }

        public string UnreconciledAmounts()
        {
            var results = transRepo.UnreconciledAmounts();
            return JsonConvert.SerializeObject(results);
        }

        public string MonthlyOverview()
        {
            //need an object with income / expenses / budgeted / next month / surplus
            var thisMonth = DateTime.Today.Month;
            var thisMonthYear = DateTime.Today.Year;
            var lastMonth = DateTime.Today.AddMonths(-1).Month;
            var lastMonthYear = DateTime.Today.AddMonths(-1).Year;

            var income = transRepo.GetAllEagerly("TransactionType")
                        .Where(t => t.TransactionDate.Year == lastMonthYear && t.TransactionDate.Month == lastMonth
                        && !t.FlagForFollowUp && t.ReimbursableSource == null && t.TransactionType.CountAsIncome==true)
                            .ToList() //this prevents null reference error when there are no transactions
                            .Sum(t=>t.Amount * (t.TransactionType==null?1:t.TransactionType.MonthlyCashflowMultiplier));

            var nextIncome = transRepo.GetAllEagerly("TransactionType")
                        .Where(t => t.TransactionDate.Year == thisMonthYear && t.TransactionDate.Month == thisMonth
                        && !t.FlagForFollowUp && t.ReimbursableSource == null && t.TransactionType.CountAsIncome == true)
                            .ToList() //this prevents null reference error when there are no transactions
                            .Sum(t => t.Amount * (t.TransactionType==null?1:t.TransactionType.MonthlyCashflowMultiplier));

            var expenses = transRepo.GetAllEagerly("TransactionType")
                        .Where(t => t.TransactionDate.Year == thisMonthYear && t.TransactionDate.Month == thisMonth
                        && !t.FlagForFollowUp && t.ReimbursableSource == null && t.TransactionType.CountAsIncome == false)
                            .ToList() //this prevents null reference error when there are no transactions
                            .Sum(t => t.Amount * (t.TransactionType==null?1:t.TransactionType.MonthlyCashflowMultiplier));

            var budgeted = budgetRepo.GetAll().Sum(b => b.BudgetedAmount);

            var vm = new
            {
                IncomeForCurrentMonth = income,
                IncomeForNextMonth = nextIncome,
                ExpensesForCurrentMonth = Math.Abs(expenses),
                BudgetedExpenses = budgeted,
                IncomeToBudgetSurplus = income - budgeted
            };

            return JsonConvert.SerializeObject(vm);
        }
    }
}