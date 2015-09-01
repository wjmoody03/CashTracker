using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class AccountDownloadResult
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalTranasctionsDownloaded { get; set; }
        public int TransactionsFlaggedAsPossibleDuplicates { get; set; }
        public int TransactionsExcludedBecauseTheAlreadyExisted { get; set; }
        public int TransactionsWithUnknownCategories { get; set; }
        public int NetNewTransactions { get; set; }
        public List<Transaction> NewTransactions { get; set; }
    }
}
