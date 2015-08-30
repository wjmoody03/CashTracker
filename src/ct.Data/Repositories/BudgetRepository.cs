using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface IBudgetRepository:IGenericRepositoryEF<BudgetCategory>
    {
        
    }

    public class BudgetRepository : GenericRepositoryEF<BudgetCategory>, IBudgetRepository
    {
        IctContext Context;

        public BudgetRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
