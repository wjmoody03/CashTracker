using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{

    public class Transaction //:TableEntity
    {
        public Transaction() {
            if (string.IsNullOrEmpty(this.TransactionID))
            {
                this.TransactionID = Guid.NewGuid().ToString();
            }
        }

        [Key]
        public int ID { get; set; }
        public string TransactionID { get; set; }
        public string UserID { get; set; }
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
        public List<Transaction> SplitTransactions { get; set; }
        public TransactionType TransactionType { get; set; }
        public Account Account { get; set; }
        public Download Download { get; set; }
        [ForeignKey("ParentTransactionID")]
        public Transaction ParentTransaction { get; set; }


    }
}
