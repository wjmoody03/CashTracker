﻿using ct.Data.Repositories;
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
        IAccountRepository accountRepo;
        ITransactionRepository transRepo;
        IAccountBalanceRepository balanceRepo;

        public BalanceCalculator(IAccountRepository AccountRepo, ITransactionRepository TransactionRepo, IAccountBalanceRepository BalanceRepo)
        {
            accountRepo = AccountRepo;
            transRepo = TransactionRepo;
            balanceRepo = BalanceRepo;
        }

        public decimal RemainingSpendableFunds(DateTime AsOfDate)
        {
            throw new NotImplementedException();
        }

        public decimal OutstandingCreditCardDebt(DateTime AsOfDate)
        {
            return balanceRepo.StatedAccountBalancesAsOf(AsOfDate)
                   .Where(b => b.Account.AccountType == AccountType.CreditCard)
                   .Sum(b => b.BalanceAmount);
        }

    }
}
