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
        public static void RemoveExistingTransactionsAndApplyFlagsToPossibleDupes(IEnumerable<Transaction> AllExistingTransactions, ref List<Transaction> AllPotentialNewTransactions)
        {

            AllPotentialNewTransactions.RemoveAll(tNew => AllExistingTransactions.Any(tOld => tOld.SourceTransactionIdentifier == tNew.SourceTransactionIdentifier));
            foreach(var t in AllPotentialNewTransactions)
            {
                if(AllExistingTransactions.Any(tOld=> tOld.Description.ToLower()==t.Description.ToLower() 
                                                        && tOld.TransactionDate == t.TransactionDate
                                                        && t.Amount == t.Amount
                                                        && t.AccountID == t.AccountID))
                {
                    t.FlagForFollowUp = true;
                    t.Notes = "This is a possible duplicate of an existing transaction with the same description, date, amount an account. " +
                        "If this is in fact a unique transaction you can clear the flag. If it is a duplicate you should delete it.";
                }
            }


        }
    }
}
