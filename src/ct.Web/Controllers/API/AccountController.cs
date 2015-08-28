using ct.Data.Repositories;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ct.Web.Controllers.API
{
    [Authorize]
    public class AccountController : ApiController
    {
        private IAccountRepository acctRepo;

        public AccountController(IAccountRepository AccountRepo)
        {
            acctRepo = AccountRepo;
        }

        // GET: api/Transactions
        public IEnumerable<Account> GetTransactions()
        {
            var acct = acctRepo.GetAll();
            return acct;
        }
    }
}
