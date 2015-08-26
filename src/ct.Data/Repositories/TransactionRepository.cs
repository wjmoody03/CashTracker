using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;
using ct.Domain;

namespace ct.Data.Repositories
{
    public interface ITransactionRepository : IGenericRepositoryEF<Transaction>
    {
        IQueryable<Transaction> TransactionsNeedingAttention();
        IEnumerable<string> ProbableCategories();
    }

    public class TransactionRepository : GenericRepositoryEF<Transaction>, ITransactionRepository
    {
        
        public TransactionRepository(IctContext cxt)
            : base(cxt)
        {
            
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
