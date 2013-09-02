using ct.Data.Repositories;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business
{
    public class BalanceCalculator
    {
        ITransactionRepository transRepo;
        IAccountBalanceRepository balanceRepo;

        public BalanceCalculator(ITransactionRepository TransactionRepo, IAccountBalanceRepository BalanceRepo)
        {
            transRepo = TransactionRepo;
            balanceRepo = BalanceRepo;
        }

        public decimal RemainingSpendableFunds(DateTime AsOfDate, bool DiscountFlaggedTransactions, bool DiscountExpectedReimbursals)
        {
            //*one potential bug with this calc: If we've sent a payment to a credit card (which has decreased our checking
            //balance) that has not yet posted to the credit card account (and thus not decreased our cc debt) we will
            //understate our available funds by the amount of the payment. 
            //*another potential mismatch is that we look at stated balances for credit card debt, not actual transactions. 
            //in the case where there is a large reconciliation issue for a credit card, this could be confusing. 
            //*Need to point out to the user on the dashboard page the amount of this that is reimbursable/flagged in case it 
            //affects tight casflow situations. These are pseudo amounts and, unless resolved/reimbursed, don't matter
            return TotalCashOnHand(AsOfDate)
                    - OutstandingCreditCardDebt(AsOfDate)
                    - IncomeReservedForFutureUse(AsOfDate)
                    - (DiscountFlaggedTransactions ? NetCashflowEffectOfTransactionsFlaggedForFollowUp() : 0)
                    - (DiscountExpectedReimbursals ? NetCashflowEffectOfReimbursableTransactions(AsOfDate) : 0);
        }

        public decimal TotalCashOnHand(DateTime AsOfDate)
        {
            return balanceRepo.StatedAccountBalancesAsOf(AsOfDate)
                   .Where(b => b.Account.AccountType == AccountType.CheckingAccount)
                   .Sum(b => b.BalanceAmount);
        }

        public decimal OutstandingCreditCardDebt(DateTime AsOfDate)
        {
            return balanceRepo.StatedAccountBalancesAsOf(AsOfDate)
                   .Where(b => b.Account.AccountType == AccountType.CreditCard)
                   .Sum(b => b.BalanceAmount);
        }

        public decimal IncomeReservedForFutureUse(DateTime AsOfDate)
        {
            return transRepo
                        .GetAllEagerly("TransactionType")
                        .Where(t => t.TransactionType.CountAsIncome == true
                                    && t.TransactionDate.Month >= AsOfDate.Month
                                    && t.TransactionDate.Year >= AsOfDate.Year
                                    && t.ReimbursableSource == null //these are included in a different category. Don't double count them here. 
                                    && t.FlagForFollowUp == false
                                )
                        .Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier);
        }

        public decimal NetCashflowEffectOfTransactionsFlaggedForFollowUp() //we don't take an as of date here because we assume all will be resolved eventually, and historical timing is irrelevant
        {
            return transRepo
                        .GetAllEagerly("TransactionType")
                        .Where(t => t.FlagForFollowUp == true)
                        .Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier);
        }

        public decimal NetCashflowEffectOfReimbursableTransactions(DateTime AsOfDate)
        {
            return transRepo
                        .GetAllEagerly("TransactionType")
                        .Where(t => t.ReimbursableSource != null && t.TransactionDate <= AsOfDate)
                        .Sum(t => t.Amount * t.TransactionType.MonthlyCashflowMultiplier);
        }

    }
}
