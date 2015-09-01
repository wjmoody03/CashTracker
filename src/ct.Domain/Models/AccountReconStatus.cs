using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class AccountReconStatus
    {
        public int AccountID { get; set; }
        public string Account { get; set; }
        public decimal CalculatedBalance { get; set; }
        public decimal StatedBalance { get; set; }
        public decimal UnreconciledAmount { get; set; }
    }
}
