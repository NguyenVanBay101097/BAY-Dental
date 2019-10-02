using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceBasic
    {
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
        /// Invoice Date, Keep empty to use the current date
        /// </summary>
        public DateTime? DateInvoice { get; set; }

        /// <summary>
        /// Partner
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Remaining amount due.
        /// </summary>
        public decimal Residual { get; set; }

        /// <summary>
        /// Salesperson
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public DateTime? DateOrder { get; set; }
    }

    public class AccountInvoiceCbx
    {
        public Guid Id { get; set; }

        public string Number { get; set; }
    }

    public class AccountInvoicePaged
    {
        public AccountInvoicePaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string SearchPartnerNamePhone { get; set; }

        public string SearchNumber { get; set; }

        public DateTime? DateInvoiceFrom { get; set; }

        public DateTime? DateInvoiceTo { get; set; }

        public DateTime? DateOrderFrom { get; set; }

        public DateTime? DateOrderTo { get; set; }

        public Guid? PartnerId { get; set; }

        public string Type { get; set; }
    }
}
