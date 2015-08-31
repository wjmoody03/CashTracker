using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ct.Web.Models
{
    public class AccountViewModel:Account
    {
        public string AccountTypeDescription { get; set; }
    }
}