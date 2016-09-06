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
            var existingSourceIdentifiers = AllExistingTransactions.Where(t=>t.SourceTransactionIdentifier!= null).ToDictionary(t => ChaseSafeTransactionIdentifier(t));
            adr.TransactionsExcludedBecauseTheyAlreadyExisted = adr.NewTransactions.RemoveAll(tNew => existingSourceIdentifiers.ContainsKey(ChaseSafeTransactionIdentifier(tNew)));

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

        private static string ChaseSafeTransactionIdentifier(Transaction t)
        {
            //on aug 4 2016 chase updated their website and started applying the same FITID to foreign transaction fees
            //as the actual transaction. Much cursing was had. 
            //this just includes a uniqueness check with that date in mind. 
            if(t.Description== "FOREIGN TRANSACTION FEE")
            {
                return t.SourceTransactionIdentifier + "FTF";
            }
            else
            {
                return t.SourceTransactionIdentifier;
            }


        }

        private static string transactionKey(Transaction t)
        {
            return t.Description.ToLower() + t.TransactionDate.ToShortDateString() + Math.Round(t.Amount,2).ToString() + t.AccountID.ToString();
        }
    }
}
