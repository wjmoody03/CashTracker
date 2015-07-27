using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ct.Domain.Models;
using ct.Data.Contexts;
using ct.Data.Repositories;

namespace ct.Web.Controllers.API
{
    public class ReimbursableSourceController : ApiController
    {
        private ctContext db = new ctContext();
        ITransactionRepository transRepo;

        public ReimbursableSourceController()
        {
            transRepo = new TransactionRepository(db);
        }

        // GET api/Transaction
        public IEnumerable<string> GetReimbursableSources()
        {
            return db.Transactions.Where(t=>t.ReimbursableSource != null).Select(t=>t.ReimbursableSource).Distinct().OrderBy(ri=>ri);
        }
        public IEnumerable<string> GetReimbursableSources(DateTime StartDate, DateTime EndDate)
        {
            return transRepo.GetAll().Where(t => t.TransactionDate >= StartDate && t.TransactionDate <= EndDate && t.ReimbursableSource != null).Select(t => t.ReimbursableSource).Distinct().OrderBy(ri => ri);
        }

    }
}