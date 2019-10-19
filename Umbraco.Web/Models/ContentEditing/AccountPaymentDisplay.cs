using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountPaymentDisplay
    {
        public Guid Id { get; set; }

        public Guid? PartnerId { get; set; }
        public PartnerBasic Partner { get; set; }

        public string PartnerType { get; set; }

        public DateTime PaymentDate { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalBasic Journal { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public string PaymentType { get; set; }

        public decimal Amount { get; set; }

        public string Communication { get; set; }
    }
}
