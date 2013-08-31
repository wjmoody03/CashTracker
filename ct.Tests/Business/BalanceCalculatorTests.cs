using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ct.Business;
using ct.Data.Repositories;
using ct.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using ct.Data.Contexts;
using ct.Tests.Data.Contexts;

namespace ct.Tests.Business
{
    [TestClass]
    public class BalanceCalculatorTests
    {

        [TestMethod]
        public void outstanding_credit_card_balance_is_calculated_correctly()
        {
            var balances = new List<AccountBalance>()
            {
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 100,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 1,
                     Account = new Account(){ AccountType = AccountType.CreditCard}
                },
                //new AccountBalance(){ //valid account, in-scope date, but not the most recent
                //     BalanceAmount = 50,
                //     BalanceDate = new DateTime(2013,6,1),
                //     AccountID = 1,
                //     Account = new Account(){ AccountType = AccountType.CreditCard}
                //},
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 30,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 2,
                     Account = new Account(){ AccountType = AccountType.CreditCard}
                },
                //new AccountBalance(){ //valid account, but too late
                //     BalanceAmount = 500,
                //     BalanceDate = new DateTime(2013,9,1),
                //     AccountID = 1,
                //     Account = new Account(){ AccountType = AccountType.CreditCard}
                //},
                new AccountBalance(){ //good date, but invalid account
                     BalanceAmount = 1000,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 3,
                     Account = new Account(){ AccountType = AccountType.CheckingAccount}
                },

            };

            var asOfDate = new DateTime(2013, 8, 1);
            var accountRepo = MockRepository.GenerateMock<IAccountRepository>();
            var transRepo = MockRepository.GenerateMock<ITransactionRepository>();
            var balanceRepo = MockRepository.GenerateMock<IAccountBalanceRepository>();
            balanceRepo.Stub(br => br.StatedAccountBalancesAsOf(asOfDate)).Return(balances);
            var bc = new BalanceCalculator(accountRepo, transRepo, balanceRepo);
            var cc = bc.OutstandingCreditCardDebt(asOfDate);
            Assert.AreEqual(130, cc);
        }
    }
}
