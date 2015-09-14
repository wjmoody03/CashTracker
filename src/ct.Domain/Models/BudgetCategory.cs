using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    [Table("Budget")]
    public class BudgetCategory
    {
        [Key][Column("ID")]
        public int BudgetCategoryID { get; set; }
        public string Category { get; set; }
        public bool VariableExpense { get; set; }
        public decimal BudgetedAmount { get; set; }
    }
}
