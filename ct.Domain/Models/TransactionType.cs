using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class TransactionType
    {
        public int TransactionTypeID { get; set; }
        public string TransactionTypeDescription { get; set; }
        public int ReconciliationMultiplier { get; set; }
        public int MonthlyCashflowMultiplier { get; set; }
        public int MonthlyCashflowMonthDifferential { get; set; }
        public bool CalcInCategoryOverview { get; set; }
        public bool CountAsIncome { get; set; }
    }
}
