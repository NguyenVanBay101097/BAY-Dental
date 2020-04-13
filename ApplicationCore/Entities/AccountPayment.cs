using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountPayment: BaseEntity
    {
        public AccountPayment()
        {
            State = "draft";
            PaymentDifferenceHandling = "open";
        }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public string PartnerType { get; set; }

        public DateTime PaymentDate { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public string PaymentType { get; set; }

        public decimal Amount { get; set; }

        public string Communication { get; set; }

        public ICollection<AccountInvoicePaymentRel> AccountInvoicePaymentRels { get; set; } = new List<AccountInvoicePaymentRel>();

        public ICollection<AccountMoveLine> MoveLines { get; set; }

        /// <summary>
        /// ('open', 'Keep open'), ('reconcile', 'Mark invoice as fully paid')
        /// </summary>
        public string PaymentDifferenceHandling { get; set; }

        public Guid? WriteoffAccountId { get; set; }
        public AccountAccount WriteoffAccount { get; set; }

        public ICollection<AccountMovePaymentRel> AccountMovePaymentRels { get; set; } = new List<AccountMovePaymentRel>();

        [NotMapped]
        public AccountAccount DestinationAccount { get; set; }
    }
}
