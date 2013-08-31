using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class AccountBalance
    {
        public int AccountBalanceID { get; set; }
        public int AccountID { get; set; }
        public int? DownloadID { get; set; }
        public DateTime BalanceDate { get; set; }
        public decimal BalanceAmount { get; set; }

        public Account Account { get; set; }

    }
}
