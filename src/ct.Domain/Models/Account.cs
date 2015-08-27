using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class Account
    {
        [Key,Column("ID")]
        public int AccountID { get; set; }
        [NotMapped]
        public AccountType AccountType { get; set; }
        [Column("Account")]
        public string AccountName { get; set; }
        public decimal StartingBalance { get; set; }

        public ICollection<AccountBalance> BalanceHistory { get; set; }
        public ICollection<Download> Downloads { get; set; }

    }
}
