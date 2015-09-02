using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface IAccountDownloadResultRepository:IGenericRepositoryEF<AccountDownloadResult>
    {
        
    }

    public class AccountDownloadResultRepository : GenericRepositoryEF<AccountDownloadResult>, IAccountDownloadResultRepository
    {
        IctContext Context;

        public AccountDownloadResultRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }
    }
}
