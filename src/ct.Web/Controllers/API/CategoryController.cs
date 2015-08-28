using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ct.Data.Contexts;
using ct.Domain.Models;
using ct.Data.Repositories;
using ct.Web.Models;

namespace ct.Web.Controllers.API
{
    [Authorize]
    public class CategoryController : ApiController
    {
        private ITransactionRepository transRepo;

        public CategoryController(ITransactionRepository TransactionRepo)
        {
            transRepo = TransactionRepo;
        }

        // GET: api/Transactions
        public IEnumerable<string> GetCategories()
        {
            var cat = transRepo.GetAll().Where(t => t.Category != null && t.Category.Trim() != "").Select(t => t.Category).Distinct().OrderBy(c => c);
            return cat;
        }
    }
}