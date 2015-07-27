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
    public class CategoryController : ApiController
    {
        private ctContext db = new ctContext();
        ITransactionRepository transRepo;

        public CategoryController()
        {
            transRepo = new TransactionRepository(db);
        }

        // GET api/Transaction
        public IEnumerable<string> GetCategories()
        {
            return db.Transactions
                    .Select(t => t.Category)
                    .Distinct()
                    .ToList()
                    .Where(c=>!string.IsNullOrWhiteSpace(c))                    
                    .OrderBy(c => c);
        }
        public IEnumerable<string> GetCategories(DateTime StartDate, DateTime EndDate)
        {
            return transRepo
                    .GetAll()
                    .Where(t => t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                    .Select(t => t.Category)
                    .Distinct()
                    .ToList()
                    .Where(c=>!string.IsNullOrWhiteSpace(c))
                    .OrderBy(c => c);
        }

    }
}