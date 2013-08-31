using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ct.Domain.Models;
using ct.Data.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace ct.Tests.Data.Repositories
{
    [TestClass]
    public class AccountBalanceRepositoryTests:RepositoryTestClass
    {
        [TestMethod]
        public void stated_balances_as_of_given_date_returns_correct_balances_by_account()
        {
            var accounts = new List<Account>(){
                new Account(){
                    AccountID = 1,
                    AccountName="fake",
                    AccountType =  AccountType.CheckingAccount
                },
                new Account(){
                    AccountID = 2,
                    AccountName="fake",
                    AccountType =  AccountType.CheckingAccount
                },
                new Account(){
                    AccountID = 3,
                    AccountName="fake",
                    AccountType =  AccountType.CheckingAccount
                },
                new Account(){
                    AccountID = 4,
                    AccountName="fake",
                    AccountType =  AccountType.CheckingAccount
                }
            };
            
            var balances = new List<AccountBalance>()
            {
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 100,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 1,
                },
                new AccountBalance(){ //valid account, in-scope date, but not the most recent
                     BalanceAmount = 50,
                     BalanceDate = new DateTime(2013,6,1),
                     AccountID = 1,
                },
                new AccountBalance(){ //valid, in-scope balance
                     BalanceAmount = 30,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 2,
                },
                new AccountBalance(){ //valid account, but too late
                     BalanceAmount = 500,
                     BalanceDate = new DateTime(2013,9,1),
                     AccountID = 2,
                },
                new AccountBalance(){ //valid balance
                     BalanceAmount = 1000,
                     BalanceDate = new DateTime(2013,7,1),
                     AccountID = 3,
                },

            };
            var ac = new AccountRepository(context);
            ac.AddRange(accounts);
            ac.Save();
            var br = new AccountBalanceRepository(context);
            br.AddRange(balances);
            br.Save();
            var x = br.StatedAccountBalancesAsOf(new DateTime(2013, 8, 1));
            Assert.AreEqual(3, x.Count());
            Assert.AreEqual(1130, x.Sum(b => b.BalanceAmount));
        }
    }
}
