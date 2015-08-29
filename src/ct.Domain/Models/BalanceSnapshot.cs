using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class BalanceSnapshot
    {
        public DateTime AsOfDate { get; set; }
        public decimal CashOnHand { get; set; } //total balance in checking accounts
        public decimal CreditCardDebt { get; set; } //total balance on credit cards
        public decimal IncomeForFutureMonths { get; set; }
        public decimal FlaggedTransactions { get; set; }
        public decimal Reimbursable { get; set; }
        public decimal CashAvailableToSpendThisMonth { get; set; }
    }
}
