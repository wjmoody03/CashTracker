using ct.Domain;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Data.Contexts
{
    public interface IctContext : IDbContext
    {
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<TransactionType> TransactionTypes { get; set; }
        DbSet<Download> Downloads { get; set; }
        DbSet<AccountBalance> AccountBalances { get; set; }
        DbSet<BudgetCategory> Budget { get; set; }
        DbSet<AccountDownloadResult> DownloadResults { get; set; }
        DbSet<Reminder> Reminders { get; set; }
    }
    public class ctContext: DbContext, IctContext
    {
        
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<AccountBalance> AccountBalances { get; set; }
        public DbSet<BudgetCategory> Budget { get; set; }
        public DbSet<AccountDownloadResult> DownloadResults { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        public ctContext(string connectionString):base(connectionString)
        {

        }
        public ctContext() : base(CashTrackerConfigurationManager.AzureSQLConnectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //deleting a download will not delete an account
            modelBuilder.Entity<Download>()
                .HasRequired(d => d.Account)
                .WithMany(a => a.Downloads)
                .HasForeignKey(d => d.AccountID)
                .WillCascadeOnDelete(false);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
