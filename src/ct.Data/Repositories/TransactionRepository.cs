using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface ITransactionRepository:IGenericRepository<Transaction>
    {
    }

    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        IctContext Context;

        public TransactionRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
