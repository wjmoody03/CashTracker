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

namespace ct.Web.Controllers.API
{
    public class TransactionController : ApiController
    {
        private ctContext db = new ctContext();

        // GET api/Transaction
        public IEnumerable<Transaction> GetTransactions()
        {
            return db.Transactions.AsEnumerable();
        }
        public IEnumerable<Transaction> GetTransactions(DateTime StartDate, DateTime EndDate)
        {
            return db.Transactions.Where(t => t.TransactionDate >= StartDate && t.TransactionDate <= EndDate) ;
        }

        // GET api/Transaction/5
        public Transaction GetTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return transaction;
        }

        // PUT api/Transaction/5
        public HttpResponseMessage PutTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != transaction.TransactionID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Transaction
        public HttpResponseMessage PostTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, transaction);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = transaction.TransactionID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Transaction/5
        public HttpResponseMessage DeleteTransaction(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Transactions.Remove(transaction);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, transaction);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}