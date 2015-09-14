using ct.Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ct.Web.Controllers
{
    public class AnalysisController : Controller
    {
        IAccountBalanceRepository acctBalanceRepo;
        IBudgetRepository budgetRepo;

        public AnalysisController(IAccountBalanceRepository AccountBalanceRepo, IBudgetRepository BudgetRepo)
        {
            acctBalanceRepo = AccountBalanceRepo;
            budgetRepo = BudgetRepo;
        }

        // GET: Analysis
        public string BalanceSnapshotHistory(DateTime StartDate, DateTime EndDate)
        {
            var balanceHistory = acctBalanceRepo.BalanceSnapshot(StartDate, EndDate);
            var categories = balanceHistory.Select(h => h.AsOfDate.ToShortDateString());
            var cashAvailable = balanceHistory.Select(h => h.CashAvailableToSpendThisMonth);
            //var cashOnHand = balanceHistory.Select(h => h.CashOnHand);
            var hc = new
            {
                chart = new {type="line"},
                title= new {text="Snapshot History"},
                xAxis = new { categories = categories },
                yAxis = new {title = new {text="$"}},
                series = new object[]
                {
                    //new { name= "Cash On Hand", data= cashOnHand }
                    new { name= "Cash Available To Spend", data= cashAvailable}
                }

            };

            return JsonConvert.SerializeObject(hc);
        }

        public string IncomeVsExpenseHistory(DateTime StartDate, DateTime EndDate)
        {
            var iveHist = acctBalanceRepo.IncomeVsExpenseHistory(StartDate, EndDate);
            var categories = iveHist.Select(h => h.EffectiveMonth.ToString("MMM yyyy"));
            var income = iveHist.Select(h => h.Income);
            var exp = iveHist.Select(h => h.Expenses);
            var hc = new
            {
                chart = new { type = "column" },
                title = new { text = "Income Vs Expenses" },
                subtitle = new { text = "*Note income is from month prior" },
                xAxis = new { categories = categories },
                yAxis = new { title = new { text = "$" } },
                series = new object[]
                {
                    new { name= "Income", data= income},
                    new { name= "Expenses", data= exp}
                }

            };

            return JsonConvert.SerializeObject(hc);
        }

        public string CategoryHistory(DateTime StartDate, DateTime EndDate)
        {
            var budget = budgetRepo.GetAll().ToDictionary(b => b.Category.ToLower());
            var catHist = acctBalanceRepo.CategoryHistory(StartDate, EndDate);
            var months = catHist.Select(h => h.MonthDate.ToString("MMM yyyy")).Distinct();
            List<HighChartSeries> series = new List<HighChartSeries>();
            foreach(var c in catHist.Select(h => h.Category).Distinct())
            {
                //add a series array for each category
                series.Add(new HighChartSeries() {
                    name = c,
                    visible = budget.ContainsKey(c.ToLower()),
                    data = catHist.Where(h => h.Category == c).Select(h => (object)h.Total)
                });
            }

        
            var hc = new
            {
                chart = new { type = "line" },
                title = new { text = "Category History" },
                //subtitle = new { text = "*Note income is from month prior" },
                xAxis = new { categories = months },
                yAxis = new { title = new { text = "$" } },
                series = series
            };

            return JsonConvert.SerializeObject(hc);
        }

        class HighChartSeries
        {
            public string name { get; set; }
            public bool visible { get; set; }
            public IEnumerable<object> data { get; set; }
        }

    }
}