using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountRegisterPayment: BaseEntity
    {
        public DateTime PaymentDate { get; set; }

        public string Communication { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        public string PartnerType { get; set; }

        public decimal Amount { get; set; }

        public string PaymentType { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public ICollection<AccountRegisterPaymentInvoiceRel> AccountRegisterPaymentInvoiceRels { get; set; } = new List<AccountRegisterPaymentInvoiceRel>();
    }
}
