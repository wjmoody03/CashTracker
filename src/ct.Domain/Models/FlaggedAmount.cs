using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class FlaggedAmount
    {
        public DateTime AsOfDate { get; set; }
        public decimal FlaggedAmountTotal { get; set; }
    }
}
