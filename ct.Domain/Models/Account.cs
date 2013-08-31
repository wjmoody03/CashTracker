using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountName { get; set; }
        public decimal StartingBalance { get; set; }

        public ICollection<AccountBalance> BalanceHistory { get; set; }
        public ICollection<Download> Downloads { get; set; }

    }
}
