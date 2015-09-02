using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class AccountDownloadResult
    {
        [Key]
        public int AccountDownloadResultID { get; set; }
        public int AccountID { get; set; }
        public decimal AccountBalance { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalTransactionsDownloaded { get; set; }
        public int TransactionsFlaggedAsPossibleDuplicates { get; set; }
        public int TransactionsExcludedBecauseTheyAlreadyExisted { get; set; }
        public int TransactionsWithUnknownCategories { get; set; }
        public int NetNewTransactions { get; set; }
        [NotMapped]
        public List<Transaction> NewTransactions { get; set; }
    }
}
