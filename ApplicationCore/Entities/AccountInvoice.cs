using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Hóa đơn điều trị, hóa đơn hoàn tiền điều trị, hóa đơn mua hàng, hóa đơn hoàn tiền mua hàng....
    /// </summary>
    public class AccountInvoice: BaseEntity
    {
        public AccountInvoice()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            DiscountType = "fixed";
        }

        /// <summary>
        /// Reference/Description
        /// </summary>
        public string Name { get; set; }

        public string Origin { get; set; }

        //('out_invoice','Customer Invoice'),
        //('in_invoice','Supplier Invoice'),
        //('out_refund','Customer Refund'),
        //('in_refund','Supplier Refund'),
        public string Type { get; set; }

        public Guid? RefundInvoiceId { get; set; }
        public AccountInvoice RefundInvoice { get; set; }

        public string Number { get; set; }

        public string MoveName { get; set; }

        //The partner reference of this invoice.
        public string Reference { get; set; }

        /// <summary>
        /// Additional Information, Ghi chú...
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// ('draft','Draft'),
        /// ('proforma','Pro-forma'),
        /// ('proforma2','Pro-forma'),
        /// ('open','Open'),
        /// ('paid','Paid'),
        /// ('cancel','Cancelled'),
        /// </summary>
        public string State { get; set; }

        public bool Sent { get; set; }

        /// <summary>
        /// Invoice Date, Keep empty to use the current date
        /// </summary>
        public DateTime? DateInvoice { get; set; }

        //Ngày đáo hạn
        public DateTime? DateDue { get; set; }

        /// <summary>
        /// Partner
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public ICollection<AccountInvoiceLine> InvoiceLines { get; set; } = new List<AccountInvoiceLine>();

        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }

        public decimal AmountTotal { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// It indicates that the invoice has been paid and the journal entry of the invoice has been reconciled with one or several journal entries of payment.
        /// </summary>
        public bool Reconciled { get; set; }

        /// <summary>
        /// Remaining amount due.
        /// </summary>
        public decimal Residual { get; set; }

        public Guid? AccountId { get; set; }
        public AccountAccount Account { get; set; }

        /// <summary>
        /// Salesperson
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal AmountTotalSigned { get; set; }
        public decimal ResidualSigned { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<AccountInvoicePaymentRel> AccountInvoicePaymentRels { get; set; }
        public ICollection<AccountInvoiceAccountMoveLineRel> PaymentMoveLines { get; set; } = new List<AccountInvoiceAccountMoveLineRel>();

        /// <summary>
        /// Accounting Date
        /// </summary>
        public DateTime? Date { get; set; }

        public decimal AmountTax { get; set; }

        public decimal AmountUntaxed { get; set; }

        //Lấy 1 số thông tin từ Sale Order đưa qua

        /// <summary>
        /// Ngày giờ điều trị
        /// </summary>
        public DateTime? DateOrder { get; set; }

        public string Note { get; set; }

        public string DiscountType { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountFixed { get; set; }

        public decimal DiscountAmount { get; set; }
    }
}
