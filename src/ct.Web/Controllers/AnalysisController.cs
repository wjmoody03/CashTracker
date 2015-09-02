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

        public AnalysisController(IAccountBalanceRepository AccountBalanceRepo)
        {
            acctBalanceRepo = AccountBalanceRepo;
        }

        // GET: Analysis
        public string AccountBalanceHistory(DateTime StartDate, DateTime EndDate)
        {
            var balanceHistory = acctBalanceRepo.BalanceHistory(StartDate, EndDate);
            return JsonConvert.SerializeObject(balanceHistory);
        }
    }
}