﻿using ct.Data.Repositories;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ct.Web.Controllers.API
{
    public class TransactionTypeController : ApiController
    {
        private ITransactionTypeRepository ttypeRepo;

        public TransactionTypeController(ITransactionTypeRepository TransactionTypeRepo)
        {
            ttypeRepo = TransactionTypeRepo;
        }

        // GET: api/Transactions
        public IEnumerable<TransactionType> GetTransactions()
        {
            var ttype = ttypeRepo.GetAll();
            return ttype;
        }
    }
}
