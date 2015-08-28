using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface ITransactionTypeRepository:IGenericRepositoryEF<TransactionType>
    {
        
    }

    public class TransactionTypeRepository : GenericRepositoryEF<TransactionType>, ITransactionTypeRepository
    {
        IctContext Context;

        public TransactionTypeRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
