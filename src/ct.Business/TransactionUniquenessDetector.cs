using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business
{
    public static class TransactionUniquenessDetector
    {
        public static void RemoveExistingTransactionsAndApplyFlagsToPossibleDupes(IEnumerable<Transaction> AllExistingTransactions, ref AccountDownloadResult adr)
        {

            //source identifier already exists in the DB. must be a dupe
            var existingSourceIdentifiers = AllExistingTransactions.Where(t=>t.SourceTransactionIdentifier!= null).ToDictionary(t => t.SourceTransactionIdentifier);
            adr.TransactionsExcludedBecauseTheAlreadyExisted = adr.NewTransactions.RemoveAll(tNew => existingSourceIdentifiers.ContainsKey(tNew.SourceTransactionIdentifier));

            var existingTransactionKeys = AllExistingTransactions.ToLookup(t => transactionKey(t));
            foreach (var t in adr.NewTransactions)
            {
                var existing = existingTransactionKeys[transactionKey(t)].FirstOrDefault();
                if (existing!= null)
                {
                    t.FlagForFollowUp = true;
                    t.Notes = string.Format("This is a possible duplicate of existing transaction ID {0} with the same description, date, amount an account. " +
                        "If this is in fact a unique transaction you can clear the flag. If it is a duplicate you should delete it.", existing.ID);
                    adr.TransactionsFlaggedAsPossibleDuplicates++;
                }
            }
        }

        private static string transactionKey(Transaction t)
        {
            return t.Description.ToLower() + t.TransactionDate.ToShortDateString() + Math.Round(t.Amount,2).ToString() + t.AccountID.ToString();
        }
    }
}
