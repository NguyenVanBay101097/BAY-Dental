using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// view from account move line and account move: account_invoice_report
    /// </summary>
    public class AccountInvoiceReport
    {
        /// <summary>
        /// line id
        /// </summary>
        public Guid Id { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public string InvoiceOrigin { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        public string InvoiceUserId { get; set; }
        public ApplicationUser InvoiceUser { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public Guid? AccountId { get; set; }
        public AccountAccount Account { get; set; }

        public decimal Quantity { get; set; }
        public decimal PriceSubTotal { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Phụ tá
        /// </summary>
        public Guid? AssistantId { get; set; }
        public Employee Assistant { get; set; }
    }
}
