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
    public class ReminderController : ApiController
    {
        private IReminderRepository reminderRepo;

        public ReminderController(IReminderRepository ReminderRepo)
        {
            reminderRepo = ReminderRepo;
        }

        // GET: api/Reminders
        public IEnumerable<Reminder> GetReminders()
        {
            var reminder = reminderRepo.GetAll();
            return reminder;
        }

        // GET: api/Reminders/5
        [ResponseType(typeof(Reminder))]
        public async Task<IHttpActionResult> GetReminder(int id)
        {
            Reminder Reminder = await reminderRepo.FindAsync(id);
            if (Reminder == null)
            {
                return NotFound();
            }

            return Ok(Reminder);
        }

        // PUT: api/Reminders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReminder(int id, Reminder Reminder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Reminder.ReminderID)
            {
                return BadRequest();
            }

            reminderRepo.Edit(Reminder);

            try
            {
                await reminderRepo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReminderExists(id))
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

        // POST: api/Reminders
        [ResponseType(typeof(Reminder))]
        public async Task<IHttpActionResult> PostReminder(Reminder Reminder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Reminder.CreateDate = DateTime.Now;
            reminderRepo.Add(Reminder);
            await reminderRepo.SaveAsync();

            return CreatedAtRoute("DefaultApi", new { id = Reminder.ReminderID }, Reminder);
        }

        // DELETE: api/Reminders/5
        [ResponseType(typeof(Reminder))]
        public async Task<IHttpActionResult> DeleteReminder(int id)
        {
            Reminder Reminder = await reminderRepo.FindAsync(id);
            if (Reminder == null)
            {
                return NotFound();
            }

            reminderRepo.Delete(Reminder);
            await reminderRepo.SaveAsync();

            return Ok(Reminder);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool ReminderExists(int id)
        {
            return reminderRepo.FindBy(e => e.ReminderID == id).Any();
        }
    }
}