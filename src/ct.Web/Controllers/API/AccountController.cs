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
using ct.Business;

namespace ct.Web.Controllers.API
{
    [Authorize]
    public class AccountsController : ApiController
    {
        private IAccountRepository acctRepo;

        public AccountsController(IAccountRepository AccountRepo)
        {
            acctRepo = AccountRepo;
        }

        // GET: api/Accounts
        public IEnumerable<Account> GetAccounts()
        {
            var acct = acctRepo.GetAll();
            foreach(var a in acct)
            {
                a.EncryptedAccountNumber = null;
                a.EncryptedPassword = null;
                a.EncryptedUserName = null;
            }
            return acct;
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> GetAccount(int id)
        {
            Account Account = await acctRepo.FindAsync(id);
            if (Account == null)
            {
                return NotFound();
            }

            Account.EncryptedAccountNumber = null;
            Account.EncryptedPassword = null;
            Account.EncryptedUserName = null;

            return Ok(Account);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAccount(int id, Account Account, bool UpdateSensitive = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Account.AccountID)
            {
                return BadRequest();
            }

            //encrypt the sensitive data: 
            if (UpdateSensitive)
            {
                Account.EncryptedUserName = Encryptor.Encrypt(Account.EncryptedUserName);
                Account.EncryptedPassword = Encryptor.Encrypt(Account.EncryptedPassword);
                Account.EncryptedAccountNumber = Encryptor.Encrypt(Account.EncryptedAccountNumber);
            }
            else
            {
                Account existing = await acctRepo.FindAsync(id);
                Account.EncryptedAccountNumber = existing.EncryptedAccountNumber;
                Account.EncryptedPassword = existing.EncryptedPassword;
                Account.EncryptedUserName = existing.EncryptedUserName;
            }

            acctRepo.Edit(Account);

            try
            {
                await acctRepo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: api/Accounts
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> PostAccount(Account Account, bool UpdateSensitive=false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UpdateSensitive)
            {
                //encrypt the sensitive data: 
                Account.EncryptedUserName = Encryptor.Encrypt(Account.EncryptedUserName);
                Account.EncryptedPassword = Encryptor.Encrypt(Account.EncryptedPassword);
                Account.EncryptedAccountNumber = Encryptor.Encrypt(Account.EncryptedAccountNumber);

            }

            acctRepo.Add(Account);
            await acctRepo.SaveAsync();

            return CreatedAtRoute("DefaultApi", new { id = Account.AccountID }, Account);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> DeleteAccount(int id)
        {
            Account Account = await acctRepo.FindAsync(id);
            if (Account == null)
            {
                return NotFound();
            }

            acctRepo.Delete(Account);
            await acctRepo.SaveAsync();

            return Ok(Account);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool AccountExists(int id)
        {
            return acctRepo.FindBy(e => e.AccountID == id).Any();
        }
    }
}