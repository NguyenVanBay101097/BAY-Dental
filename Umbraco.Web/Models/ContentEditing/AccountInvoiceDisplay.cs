using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceDisplay
    {
        public AccountInvoiceDisplay()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            DiscountType = "fixed";
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Reference/Description
        /// </summary>
        public string Name { get; set; }

        //('out_invoice','Customer Invoice'),
        //('in_invoice','Supplier Invoice'),
        //('out_refund','Customer Refund'),
        //('in_refund','Supplier Refund'),
        public string Type { get; set; }

        public string Number { get; set; }

        /// <summary>
        /// ('draft','Draft'),
        /// ('proforma','Pro-forma'),
        /// ('proforma2','Pro-forma'),
        /// ('open','Open'),
        /// ('paid','Paid'),
        /// ('cancel','Cancelled'),
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Partner
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public IEnumerable<AccountInvoiceLineDisplay> InvoiceLines { get; set; } = new List<AccountInvoiceLineDisplay>();

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTax { get; set; }

        public decimal? AmountTotal { get; set; }

        public Guid JournalId { get; set; }

        /// <summary>
        /// Remaining amount due.
        /// </summary>
        public decimal Residual { get; set; }

        public Guid? AccountId { get; set; }

        /// <summary>
        /// Salesperson
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// Accounting Date
        /// </summary>
        public DateTime? Date { get; set; }

        public DateTime? DateOrder { get; set; }

        public DateTime? DateInvoice { get; set; }

        public string Note { get; set; }

        public string DiscountType { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountFixed { get; set; }
        public string Comment { get; set; }
    }
}
