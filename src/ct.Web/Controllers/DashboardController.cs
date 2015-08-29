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
    public class DashboardController : Controller
    {
        public IAccountBalanceRepository balanceRepo { get; set; }

        public DashboardController(IAccountBalanceRepository BalanceRepo)
        {
            balanceRepo = BalanceRepo;
        }

        public string SnapshotHistory(DateTime? StartDate = null, DateTime? EndDate = null) 
        {
            if (StartDate == null) StartDate = DateTime.Today;
            if (EndDate == null) EndDate = DateTime.Today;

            var hist = balanceRepo.BalanceSnapshot((DateTime)StartDate, (DateTime)EndDate);
            return JsonConvert.SerializeObject(hist);
        }
    }
}