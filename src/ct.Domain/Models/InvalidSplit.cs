using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class InvalidSplit
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal SplitAmount { get; set; }
        public decimal Diff { get; set; }
    }
}
