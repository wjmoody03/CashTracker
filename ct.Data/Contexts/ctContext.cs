using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Data.Contexts
{

    public class ctContext:DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<AccountBalance> AccountBalances { get; set; }

    }
}
