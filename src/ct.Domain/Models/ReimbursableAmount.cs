using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class ReimbursableAmount
    {
        public DateTime AsOfDate { get; set; }
        public decimal ReimbursableAmountTotal { get; set; }
    }
}
