using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class IncomeVsExpense
    {
        public DateTime EffectiveMonth { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
    }
}
