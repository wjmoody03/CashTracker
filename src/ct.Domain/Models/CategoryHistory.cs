using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class CategoryHistory
    {
        public DateTime MonthDate { get; set; }
        public string Category { get; set; }
        public decimal Total { get; set; }
    }
}
