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
    public class TransactionDownloader
    {
        Account account;
        ITransactionRepository transactionRepo;

        public TransactionDownloader(Account Account, ITransactionRepository TransactionRepo)
        {
            account = Account;
            transactionRepo = TransactionRepo;
        }

        public AccountDownloadResult GetAllTransactions()
        {
            var adr = new AccountDownloadResult();
            adr.AccountID = account.AccountID;
            adr.StartTime = DateTime.Now;
            var downloadStartDate = account.LastImport == null ? new DateTime(1970, 1, 1) : ((DateTime)account.LastImport).AddDays(-14);
            var ofx = CreditCardTransactionRequest.GetOFX(account.OFXUrl,
                    UserName: Encryptor.Decrypt(account.EncryptedUserName),
                    Password: Encryptor.Decrypt(account.EncryptedPassword),
                    AccountNumber: Encryptor.Decrypt(account.EncryptedAccountNumber),
                    StartDate: downloadStartDate);

            var parser = new OFXParser(ofx);
            var downloadedTransactions = parser.GetTransactions();
            adr.TotalTransactionsDownloaded = downloadedTransactions.Count();
            adr.AccountBalance = parser.GetOutstandingBalance();

            adr.NewTransactions = (from p in downloadedTransactions
                                   select new Transaction()
                               {
                                   AccountID = account.AccountID,
                                   Amount = p.TRNAMT,
                                   Description = p.NAME,
                                   SourceTransactionIdentifier = p.FITID,
                                   TransactionDate = p.DTPOSTED,
                                   TransactionTypeID = TransactionDownloader.TransactionTypeIDFromTypeAndDescription(p.TRNTYPE, p.NAME, (AccountType)Enum.Parse(typeof(AccountType),account.AccountType,true))
                               }).ToList();

            var earliestTransactionDownloaded = adr.NewTransactions.Min(t => t.TransactionDate);
            var allTrans = transactionRepo.GetAll().Where(t => t.TransactionDate >= earliestTransactionDownloaded);
            TransactionUniquenessDetector.RemoveExistingTransactionsAndApplyFlagsToPossibleDupes(allTrans, ref adr);
            CategoryGuesser.ApplyCategories(transactionRepo.CategoryGuesses(), ref adr);
            adr.NetNewTransactions = adr.NewTransactions.Count;
            adr.EndTime = DateTime.Now;
            return adr;
            
        }

        public static int TransactionTypeIDFromTypeAndDescription(string tranType, string description, AccountType accountType)
        {
            if (accountType == AccountType.CreditCard)
            {
                if (tranType == "DEBIT")
                    return (int)TransactionTypes.CreditCardExpense;

                if (tranType == "CREDIT" && description.ToLower().Contains("payment"))
                    return (int)TransactionTypes.PaymentPostedToCreditCard;

                if (tranType == "CREDIT")
                    return (int)TransactionTypes.CreditReturn;
            }

            if (accountType == AccountType.CheckingAccount)
            {
                if (tranType == "DEBIT" || tranType == "CHECK")
                    return (int)TransactionTypes.CheckingExpense;

                if (tranType == "CREDIT" && description.ToLower().Contains("CHASE DES:AUTOPAY"))
                    return (int)TransactionTypes.PaymentSentToCreditCard;

                if (tranType == "CREDIT")
                    return (int)TransactionTypes.CheckingDeposit;
            }

            return (int)TransactionTypes.Unknown;

        }
    }
}
