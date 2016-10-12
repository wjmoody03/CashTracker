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
            var allWithIdentifiers = AllExistingTransactions.Where(t => t.SourceTransactionIdentifier != null);
            var potentialDupes = allWithIdentifiers.ToLookup(t => ChaseSafeTransactionIdentifier(t)).Where(t => t.Count() > 1);
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
            //UPDATE Oct 2016. They now dupe FITIDs for transactions made at same merchant on same day. getting a new card now 
            return UniqueifyTransactionIdentifier(t.Description, t.SourceTransactionIdentifier, t.Amount);


        }
        public static string UniqueifyTransactionIdentifier(string Description, string FITID, decimal Amount)
        {
            //on aug 4 2016 chase updated their website and started applying the same FITID to foreign transaction fees
            //as the actual transaction. Much cursing was had. 
            //this just includes a uniqueness check with that date in mind. 
            if (Description == "FOREIGN TRANSACTION FEE")
            {
                return FITID + "FTF";
            }
            else
            {
                return FITID;
            }


        }

        private static string transactionKey(Transaction t)
        {
            return t.Description.ToLower() + t.TransactionDate.ToShortDateString() + Math.Round(t.Amount,2).ToString() + t.AccountID.ToString();
        }
    }
}
