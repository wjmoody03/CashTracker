using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ct.Web.Models
{
    public class TransactionViewModel:Transaction
    {
        public string Month { get; set; }
    }
}