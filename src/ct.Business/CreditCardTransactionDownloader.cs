using ct.Business.OFX.Parsers;
using ct.Business.OFX.Request;
using ct.Data.Repositories;
using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business
{
    public class CreditCardTransactionDownloader
    {
        Account account;
        ITransactionRepository transactionRepo;

        public CreditCardTransactionDownloader(Account Account, ITransactionRepository TransactionRepo)
        {
            account = Account;
            transactionRepo = TransactionRepo;
        }

        public IEnumerable<ct.Domain.Models.Transaction> GetAllTransactions()
        {
            var ofx = CreditCardTransactionRequest.GetOFX(account.OFXUrl,
                    UserName: Encryptor.Decrypt(account.EncrypedUserName),
                    Password: Encryptor.Decrypt(account.EncryptedPassword),
                    AccountNumber: Encryptor.Decrypt(account.EncryptedAccountNumber));

            var parser = new ChaseParser(ofx).GetTransactions();

            var transactions = from p in parser
                               select new Transaction()
                               {
                                   AccountID = account.AccountID,
                                   Amount = p.TRNAMT,
                                   Description = p.NAME,
                                   SourceTransactionIdentifier = p.FITID,
                                   TransactionDate = p.DTPOSTED,
                                   TransactionTypeID = TransactionTypeIDFromTypeAndDescription(p.TRNTYPE, p.NAME)
                               };

            CategoryGuesser.ApplyCategories(transactionRepo.CategoryGuesses(), ref transactions);

            return transactions;
            
        }

        private int TransactionTypeIDFromTypeAndDescription(string tranType, string description)
        {
            if (tranType == "DEBIT")
                return (int)TransactionTypes.CreditCardExpense;

            if (tranType == "CREDIT" && description.ToLower().Contains("payment"))
                return (int)TransactionTypes.PaymentPostedToCreditCard;

            if (tranType == "CREDIT" && description.ToLower().Contains("payment"))
                return (int)TransactionTypes.CreditReturn;

            return (int)TransactionTypes.Unknown;

        }
    }
}
