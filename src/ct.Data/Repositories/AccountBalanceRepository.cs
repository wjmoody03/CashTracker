using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;

namespace ct.Data.Repositories
{
    public interface IAccountBalanceRepository:IGenericRepository<AccountBalance>
    {
        IEnumerable<AccountBalance> StatedAccountBalancesAsOf(DateTime AsOfDate);
    }

    public class AccountBalanceRepository : GenericRepository<AccountBalance>, IAccountBalanceRepository
    {
        IctContext Context;

        public AccountBalanceRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }

        public IEnumerable<AccountBalance> StatedAccountBalancesAsOf(DateTime AsOfDate)
        {
            return from b in GetAllEagerly("Account")
                           where b.BalanceDate <= AsOfDate
                           orderby b.BalanceDate descending
                           group b by b.Account into bh
                           select bh.FirstOrDefault();
        }
    }
}
