using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountMove: BaseEntity
    {
        public AccountMove()
        {
            Name = "/";
            State = "draft";
            Date = DateTime.Today;
            Type = "entry";
        }

        /// <summary>
        /// Number 
        /// </summary>
        public string Name { get; set; }

        public string Ref { get; set; }

        public DateTime Date { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        public string State { get; set; }
        public ICollection<AccountMoveLine> Lines { get; set; } = new List<AccountMoveLine>();

        /// <summary>
        /// Internal Note
        /// </summary>
        public string Narration { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// entry: Journal Entry
        /// out_invoice: Customer Invoice
        /// out_refund: Customer Credit Note
        /// in_invoice: Vendor Bill
        /// in_refund: Vendor Credit Note
        /// out_receipt: Sales Receipt
        /// in_receipt: Purchase Receipt
        /// </summary>
        public string Type { get; set; }

        public string InvoiceOrigin { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTax { get; set; }

        public decimal? AmountTotal { get; set; }

        public decimal? AmountResidual { get; set; }

        public decimal? AmountUntaxedSigned { get; set; }

        public decimal? AmountTaxSigned{ get; set; }

        public decimal? AmountTotalSigned { get; set; }

        public decimal? AmountResidualSigned { get; set; }

        public string InvoicePaymentState { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string InvoicePaymentRef { get; set; }

        public string InvoiceUserId { get; set; }
        public ApplicationUser InvoiceUser { get; set; }

        [NotMapped]
        public ICollection<AccountMoveLine> InvoiceLines { get; set; } = new List<AccountMoveLine>();
    }
}
