using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;
using ct.Domain;
using System.Data.SqlClient;

namespace ct.Data.Repositories
{
    public interface ITransactionRepository : IGenericRepositoryEF<Transaction>
    {
        IQueryable<Transaction> TransactionsNeedingAttention();
        IEnumerable<string> ProbableCategories();
        IEnumerable<CategoryVsBudget> CategoryExpensesVsBudget(int month, int year);
    }

    public class TransactionRepository : GenericRepositoryEF<Transaction>, ITransactionRepository
    {
        IctContext context;
        
        public TransactionRepository(IctContext cxt)
            : base(cxt)
        {
            context = cxt;
        }

        public IEnumerable<CategoryVsBudget> CategoryExpensesVsBudget(int month, int year)
        {
            var sql = EmbeddedSQL.SQL("CategoryVsBudget");
            var sMonth = new SqlParameter("@month", month);
            var sYear = new SqlParameter("@year", year);
            var results = context.Database.SqlQuery<CategoryVsBudget>(sql, sMonth, sYear);
            return results;
        }

        public IEnumerable<string> ProbableCategories()
        {
            var threeMonthsAgo = DateTime.Today.AddMonths(-3);
            var allCategories = base.GetAll()
                        .Where(t => t.TransactionDate >= threeMonthsAgo && t.Category !=null && t.Category.Trim()!="")
                        .GroupBy(t => t.Category)
                        .Select(t => new { Category = t.Key, Frequency = t.Count() })
                        .OrderByDescending(t => t.Frequency)
                        .Select(t => t.Category)
                        .Distinct();

            return allCategories;
        }

        public IQueryable<Transaction> TransactionsNeedingAttention()
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            return base.GetAll().Where(t => (t.Category==null || t.Category.Trim()=="")
                && t.FlagForFollowUp !=true
                && t.TransactionDate >= sixMonthsAgo
                && t.TransactionTypeID!=4 && t.TransactionTypeID!=5);
        }
    }
}
