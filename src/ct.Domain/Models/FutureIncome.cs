using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class FutureIncome
    {
        public DateTime AsOfDate { get; set; }
        public decimal IncomeForNextMonth { get; set; }
    }
}
