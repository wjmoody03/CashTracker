using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class BalanceHistory
    {
        public int AccountID { get; set; }
        public string AccountType { get; set; }
        public DateTime AsOfDate { get; set; }
        public decimal Balance { get; set; }
    }
}
