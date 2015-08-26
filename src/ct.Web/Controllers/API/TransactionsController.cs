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
        public IQueryable<Transaction> GetTransactions()
        {
            var dt = DateTime.Today.AddMonths(-2);
            return transRepo.GetAll().Where(t=>t.TransactionDate>= dt);
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