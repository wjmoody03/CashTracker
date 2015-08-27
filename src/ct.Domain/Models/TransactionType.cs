using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class TransactionType
    {
        [Key,Column("ID")]
        public int TransactionTypeID { get; set; }
        [Column("TransactionType")]
        public string TransactionTypeDescription { get; set; }
        public int ReconciliationMultiplier { get; set; }
        public int MonthlyCashflowMultiplier { get; set; }
        public int MonthlyCashflowMonthDifferential { get; set; }
        public int? DisplayMultiplier { get; set; }
        public bool CalcInCategoryOverview { get; set; }
        public bool CountAsIncome { get; set; }
    }
}
