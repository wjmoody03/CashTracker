using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ct.Data.Contexts;
using ct.Domain.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ct.Data.Repositories
{
    public interface IAccountBalanceRepository:IGenericRepositoryEF<AccountBalance>
    {
        List<BalanceHistory> BalanceHistory(DateTime StartDate, DateTime EndDate, AccountType? AccountType = null);
        List<IncomeVsExpense> IncomeVsExpenseHistory(DateTime StartDate, DateTime EndDate);
        IEnumerable<BalanceSnapshot> BalanceSnapshot(DateTime StartDate, DateTime EndDate);
    }

    public class AccountBalanceRepository : GenericRepositoryEF<AccountBalance>, IAccountBalanceRepository
    {
        IctContext Context;

        public AccountBalanceRepository(IctContext context)
            : base(context)
        {
            Context = context;
        }

        public List<BalanceHistory> BalanceHistory(DateTime StartDate, DateTime EndDate, AccountType? AccountType = null)
        {
            var sql = EmbeddedSQL.SQL("AccountBalanceHistory");
            var spStart = new SqlParameter("@StartDate", StartDate);
            var spEnd = new SqlParameter("@EndDate", EndDate);
            var spAcctType = new SqlParameter("@AccountType",System.Data.SqlDbType.Int);
            if(AccountType== null)
            {
                spAcctType.Value = DBNull.Value;
            }
            else
            {
                spAcctType.Value = (int)AccountType;
            }

            Context.Database.CommandTimeout = 0;
            var balances = Context.Database.SqlQuery<BalanceHistory>(sql,spStart,spEnd,spAcctType).ToList();
            return balances;
        }

        public List<FutureIncome> FutureIncomeHistory(DateTime StartDate, DateTime EndDate)
        {
            var sql = EmbeddedSQL.SQL("FutureIncomeHistory");
            var spStart = new SqlParameter("@StartDate", StartDate);
            var spEnd = new SqlParameter("@EndDate", EndDate);
            var income = Context.Database.SqlQuery<FutureIncome>(sql, spStart, spEnd).ToList();
            return income;
        }

        public List<FlaggedAmount> FlaggedAmountHistory(DateTime StartDate, DateTime EndDate)
        {
            var sql = EmbeddedSQL.SQL("FlaggedAmountHistory");
            var spStart = new SqlParameter("@StartDate", StartDate);
            var spEnd = new SqlParameter("@EndDate", EndDate);
            var amounts = Context.Database.SqlQuery<FlaggedAmount>(sql, spStart, spEnd).ToList();
            return amounts;
        }

        public List<ReimbursableAmount> ReimbursableAmountHistory(DateTime StartDate, DateTime EndDate)
        {
            var sql = EmbeddedSQL.SQL("ReimbursableAmountHistory");
            var spStart = new SqlParameter("@StartDate", StartDate);
            var spEnd = new SqlParameter("@EndDate", EndDate);
            var amounts = Context.Database.SqlQuery<ReimbursableAmount>(sql, spStart, spEnd).ToList();
            return amounts;
        }

        public IEnumerable<BalanceSnapshot> BalanceSnapshot(DateTime StartDate, DateTime EndDate)
        {
            var allBalances = BalanceHistory(StartDate, EndDate);
            var futureIncome = FutureIncomeHistory(StartDate, EndDate);
            var flaggedAmounts = FlaggedAmountHistory(StartDate, EndDate);
            var reimbursableAmounts = ReimbursableAmountHistory(StartDate, EndDate);

            //what's faster? getting all balances into memory then aggregating by account type, or running 2 separate queries? 
            var checkingBalances = aggregateBalancesAsOfDateForAccountType(allBalances, AccountTypes.Checking);
            var creditBalances = aggregateBalancesAsOfDateForAccountType(allBalances, AccountTypes.Credit);

            //now project the actual balance calc: 
            var snaps = from chk in checkingBalances
                            join cdt in creditBalances on chk.BalanceDate equals cdt.BalanceDate
                            join fi in futureIncome on chk.BalanceDate equals fi.AsOfDate
                            join fa in flaggedAmounts on chk.BalanceDate equals fa.AsOfDate
                            join ra in reimbursableAmounts on chk.BalanceDate equals ra.AsOfDate
                            select new BalanceSnapshot()
                            {
                                AsOfDate = chk.BalanceDate,
                                CashOnHand = chk.Balance,
                                CreditCardDebt = cdt.Balance,
                                FlaggedTransactions = fa.FlaggedAmountTotal,
                                IncomeForFutureMonths = fi.IncomeForNextMonth,
                                Reimbursable = ra.ReimbursableAmountTotal,
                                CashAvailableToSpendThisMonth = chk.Balance - cdt.Balance - fi.IncomeForNextMonth - fa.FlaggedAmountTotal - ra.ReimbursableAmountTotal
                            };

            return snaps;            

        }

        private IEnumerable<trackingBalance> aggregateBalancesAsOfDateForAccountType(IEnumerable<BalanceHistory> balances, string accountType)
        {
            return from b in balances
                   where b.AccountType == accountType
                   group b by b.AsOfDate into bd
                   select new trackingBalance
                   {
                       BalanceDate = bd.Key,
                       Balance = bd.Sum(bdi => bdi.Balance)
                   };
        }

        private string DateKey(DateTime StartDate, DateTime EndDate)
        {
            return StartDate.ToShortDateString() + EndDate.ToShortDateString();
        }

        public List<IncomeVsExpense> IncomeVsExpenseHistory(DateTime StartDate, DateTime EndDate)
        {
            var sql = EmbeddedSQL.SQL("IncomeVsExpenseHistory");
            var spStart = new SqlParameter("@StartDate", StartDate);
            var spEnd = new SqlParameter("@EndDate", EndDate);           
            var hist = Context.Database.SqlQuery<IncomeVsExpense>(sql, spStart, spEnd).ToList();
            return hist;
        }

        class trackingBalance
        {
            public DateTime BalanceDate { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
