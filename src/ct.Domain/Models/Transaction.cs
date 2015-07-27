using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class Transaction
    {
        [Column("ID")]
        public int TransactionID { get; set; }
        [Column("TransactionType")]
        public int TransactionTypeID { get; set; }
        public int AccountID { get; set; }
        public int? DownloadID { get; set; }
        public string SourceTransactionIdentifier { get; set; } //this is the transID of the bank or card company
        public int? ParentTransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public string ReimbursableSource { get; set; }
        public string TaxTag { get; set; }
        public bool FlagForFollowUp { get; set; }
        
        [ForeignKey("ParentTransactionID")]
        public ICollection<Transaction> SplitTransactions { get; set; }

        public TransactionType TransactionType { get; set; }
        public Account Account { get; set; }
        public Download Download { get; set; }
        [ForeignKey("TransactionID")]
        public Transaction ParentTransaction { get; set; }


    }
}
