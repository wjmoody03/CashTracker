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
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 30,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 2,
                     Account = new Account(){ AccountType = AccountType.CreditCard}
                },
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

        [TestMethod]
        public void total_cash_on_hand_is_calculated_correctly()
        {
            var balances = new List<AccountBalance>()
            {
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 100,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 1,
                     Account = new Account(){ AccountType = AccountType.CheckingAccount}
                },
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 30,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 2,
                     Account = new Account(){ AccountType = AccountType.CheckingAccount}
                },
                new AccountBalance(){ //good date, but invalid account
                     BalanceAmount = 1000,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 3,
                     Account = new Account(){ AccountType = AccountType.CreditCard}
                },

            };

            var asOfDate = new DateTime(2013, 8, 1);
            var accountRepo = MockRepository.GenerateMock<IAccountRepository>();
            var transRepo = MockRepository.GenerateMock<ITransactionRepository>();
            var balanceRepo = MockRepository.GenerateMock<IAccountBalanceRepository>();
            balanceRepo.Stub(br => br.StatedAccountBalancesAsOf(asOfDate)).Return(balances);
            var bc = new BalanceCalculator(accountRepo, transRepo, balanceRepo);
            var cc = bc.TotalCashOnHand(asOfDate);
            Assert.AreEqual(130, cc);
        }

        [TestMethod]
        public void balance_not_available_for_spending_includes_correct_transactions()
        {
            var cd = new TransactionType(){ CountAsIncome=true,MonthlyCashflowMultiplier=1 }; //checking deposit
            var cce = new TransactionType(){ CountAsIncome=false,MonthlyCashflowMultiplier=-1}; //credit card expense. should be ignored            

            var trans = new List<Transaction>(){
                new Transaction(){ //deposit for future month, should be included by function. 
                     TransactionDate = new DateTime(2013,9,1),
                     TransactionType = cd,
                     Amount = 1
                },
                new Transaction(){ //deposit during current month, but reserved for future use. should be included by function.
                     TransactionDate = new DateTime(2013,8,1),
                     TransactionType = cd,
                     Amount = 10
                },
                new Transaction(){ //deposit during prior month. This should be ignored by function. 
                     TransactionDate = new DateTime(2013,7,1),
                     TransactionType = cd,
                     Amount = 30
                },
                new Transaction(){ //deposit during prior month. this should be ignored by function
                     TransactionDate = new DateTime(2013,7,1),
                     TransactionType = cd,
                     Amount=50
                },
                new Transaction(){ //deposit during prior month, but flagged for follow up. Should be included by function
                     TransactionDate = new DateTime(2013,6,1),
                     TransactionType = cd,
                     Amount = 100,
                     FlagForFollowUp = true
                },
                new Transaction(){ //deposit during prior month, but reimbursable. should be included by function
                     TransactionDate = new DateTime(2013,9,1),
                     TransactionType = cd,
                     Amount = 200,
                     ReimbursableSource = "Dad"
                }
            };

            var asOfDate = new DateTime(2013, 8, 1);
            var accountRepo = MockRepository.GenerateMock<IAccountRepository>();
            var transRepo = MockRepository.GenerateMock<ITransactionRepository>();
            transRepo.Stub(tr => tr.GetAllEagerly("TransactionType")).Return(trans.AsQueryable());
            var balanceRepo = MockRepository.GenerateMock<IAccountBalanceRepository>();
            var bc = new BalanceCalculator(accountRepo, transRepo, balanceRepo);
            var ineligibleBalance = bc.BalanceNotAvailableForSpending(asOfDate);
            Assert.AreEqual(311, ineligibleBalance);

        }
    }
}
