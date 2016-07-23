using ct.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ct.Web.Models
{
    public class TransactionViewModel
    {
        public int ID { get; set; }
        public string TransactionID { get; set; }
        public string UserID { get; set; }
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
        public TransactionType TransactionType { get; set; }
        public Account Account { get; set; }
        public Download Download { get; set; }

        public string Month { get; set; }
        public string TransactionTypeDescription { get; set; }
        public string AccountName { get; set; }
        public bool HasBeenSplit { get; set; }
    }
}