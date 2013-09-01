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
        public void income_reserved_for_future_use_includes_correct_transactions()
        {
            var cd = new TransactionType(){ CountAsIncome=true,MonthlyCashflowMultiplier=1 }; //checking deposit
            var cce = new TransactionType(){ CountAsIncome=false,MonthlyCashflowMultiplier=-1}; //credit card expense. should be ignored            

            var trans = new List<Transaction>(){
                new Transaction(){
                    TransactionType = cce,
                    TransactionDate = new DateTime(2013,7,1),
                    Amount= 500
                },
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
                new Transaction(){ //deposit during future month, but flagged for follow up. Should not be included by function
                     TransactionDate = new DateTime(2013,9,1),
                     TransactionType = cd,
                     Amount = 100,
                     FlagForFollowUp = true
                },
                new Transaction(){ //deposit during future month, but reimbursable. should not be included by function
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
            var ineligibleBalance = bc.IncomeReservedForFutureUse(asOfDate);
            Assert.AreEqual(11, ineligibleBalance);

        }


        [TestMethod]
        public void cashflow_effect_of_flagged_transactions_calculates_correctly()
        {
            var cd = new TransactionType() { CountAsIncome = true, MonthlyCashflowMultiplier = 1 }; //checking deposit
            var cce = new TransactionType() { CountAsIncome = false, MonthlyCashflowMultiplier = -1 }; //credit card expense. should be ignored            

            var trans = new List<Transaction>(){
                new Transaction(){
                    TransactionType = cce,
                    Amount= 500,
                    FlagForFollowUp = true
                },
                new Transaction(){ //deposit for future month, should be included by function. 
                     TransactionType = cd,
                     Amount = 1,
                     FlagForFollowUp = false
                },
                new Transaction(){ //deposit during current month, but reserved for future use. should be included by function.
                     TransactionType = cd,
                     Amount = 10,
                     FlagForFollowUp = true
                }
            };

            var accountRepo = MockRepository.GenerateMock<IAccountRepository>();
            var transRepo = MockRepository.GenerateMock<ITransactionRepository>();
            transRepo.Stub(tr => tr.GetAllEagerly("TransactionType")).Return(trans.AsQueryable());
            var balanceRepo = MockRepository.GenerateMock<IAccountBalanceRepository>();
            var bc = new BalanceCalculator(accountRepo, transRepo, balanceRepo);
            var bal = bc.NetCashflowEffectOfTransactionsFlaggedForFollowUp();
            Assert.AreEqual(-490, bal);

        }

        [TestMethod]
        public void cashflow_effect_of_reimbursable_transactions_calculates_correctly()
        {
            var cd = new TransactionType() { CountAsIncome = true, MonthlyCashflowMultiplier = 1 }; //checking deposit
            var cce = new TransactionType() { CountAsIncome = false, MonthlyCashflowMultiplier = -1 }; //credit card expense. should be ignored            

            var trans = new List<Transaction>(){
                new Transaction(){ 
                    TransactionType = cce,
                    Amount= 500,
                    TransactionDate = new DateTime(2013,7,1),
                    ReimbursableSource = "Hi"
                },
                new Transaction(){ //this is in the future... we want to ignore it
                     TransactionType = cd,
                     Amount = 1,
                     ReimbursableSource = "hi",
                     TransactionDate = new DateTime(2013,9,1)
                },
                new Transaction(){ //not flagged. ignore it
                     TransactionType = cd,
                     Amount = 10,
                     ReimbursableSource = null,
                     TransactionDate = new DateTime(2013,7,1)
                },
                new Transaction(){ //valid, but CD instead of CCE
                     TransactionType = cd,
                     Amount = 25,
                     ReimbursableSource = "hi",
                     TransactionDate = new DateTime(2013,7,1)
                }
            };

            var accountRepo = MockRepository.GenerateMock<IAccountRepository>();
            var transRepo = MockRepository.GenerateMock<ITransactionRepository>();
            transRepo.Stub(tr => tr.GetAllEagerly("TransactionType")).Return(trans.AsQueryable());
            var balanceRepo = MockRepository.GenerateMock<IAccountBalanceRepository>();
            var bc = new BalanceCalculator(accountRepo, transRepo, balanceRepo);
            var bal = bc.NetCashflowEffectOfReimbursableTransactions(new DateTime(2013,8,1));
            Assert.AreEqual(-475, bal);

        }
    }
}
