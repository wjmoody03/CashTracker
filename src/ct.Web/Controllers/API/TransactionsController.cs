﻿using System;
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
    public class TransactionsController : ApiController
    {
        private ITransactionRepository transRepo;

        public TransactionsController(ITransactionRepository TransactionRepo)
        {
            transRepo = TransactionRepo;
        }

        // GET: api/Transactions
        public IEnumerable<TransactionViewModel> GetTransactions(DateTime StartDate, DateTime EndDate)
        {
            //var dt = DateTime.Today.AddMonths(-2);
            var trans = transRepo.GetAllEagerly("TransactionType","Account").Where(t=>t.TransactionDate>= StartDate.Date && t.TransactionDate <= EndDate.Date).OrderByDescending(t=>t.TransactionDate).AsEnumerable();
            var mapped = AutoMapper.Mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionViewModel>>(trans);

            //now let's identify which ones have been split:
            var ParentIdsOfSplitTransactions = transRepo.GetAll().Select(t => t.ParentTransactionID??0).Distinct().ToDictionary(t=>t);
            foreach(var t in mapped)
            {
                t.HasBeenSplit = ParentIdsOfSplitTransactions.ContainsKey(t.ID);
            }
            return mapped;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransaction(int id)
        {
            Transaction transaction = await transRepo.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            transaction.SplitTransactions = transRepo.FindByNoTracking(t => t.ParentTransactionID == id).ToList();
            return Ok(transaction);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.ID)
            {
                return BadRequest();
            }

            transRepo.Edit(transaction);

            try
            {
                await transRepo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            transRepo.Add(transaction);
            await transRepo.SaveAsync();

            return CreatedAtRoute("DefaultApi", new { id = transaction.ID }, transaction);
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await transRepo.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            transRepo.Delete(transaction);
            await transRepo.SaveAsync();

            return Ok(transaction);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool TransactionExists(int id)
        {
            return transRepo.FindBy(e => e.ID == id).Any();
        }
    }
}