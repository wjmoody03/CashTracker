using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class Reminder
    {
        [Key]
        public int ReminderID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Message { get; set; }
    }
}
