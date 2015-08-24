using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface IAccountRepository:IGenericRepositoryEF<Account>
    {
        
    }

    public class AccountRepository : GenericRepositoryEF<Account>, IAccountRepository
    {
        IctContext Context;

        public AccountRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
