using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class CategoryVsBudget
    {
        public string Category { get; set; }
        public decimal Spent { get; set; }
        public decimal Budgeted { get; set; }
    }
}
