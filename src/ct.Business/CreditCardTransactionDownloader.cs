﻿using ct.Business.OFX.Parsers;
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

        public AccountDownloadResult GetAllTransactions()
        {
            var adr = new AccountDownloadResult();
            var downloadStartDate = account.LastDownload == null ? new DateTime(1970, 1, 1) : ((DateTime)account.LastDownload).AddDays(-14);
            var ofx = CreditCardTransactionRequest.GetOFX(account.OFXUrl,
                    UserName: Encryptor.Decrypt(account.EncryptedUserName),
                    Password: Encryptor.Decrypt(account.EncryptedPassword),
                    AccountNumber: Encryptor.Decrypt(account.EncryptedAccountNumber),
                    StartDate: downloadStartDate);

            var parser = new ChaseParser(ofx).GetTransactions();
            adr.TotalTranasctionsDownloaded = parser.Count();

            var transactions = (from p in parser
                               select new Transaction()
                               {
                                   AccountID = account.AccountID,
                                   Amount = p.TRNAMT,
                                   Description = p.NAME,
                                   SourceTransactionIdentifier = p.FITID,
                                   TransactionDate = p.DTPOSTED,
                                   TransactionTypeID = TransactionTypeIDFromTypeAndDescription(p.TRNTYPE, p.NAME)
                               }).ToList();

            var allTrans = transactionRepo.GetAll().Where(t => t.TransactionDate >= downloadStartDate);
            TransactionUniquenessDetector.RemoveExistingTransactionsAndApplyFlagsToPossibleDupes(allTrans, ref adr);
            CategoryGuesser.ApplyCategories(transactionRepo.CategoryGuesses(), ref adr);
            return adr;
            
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
