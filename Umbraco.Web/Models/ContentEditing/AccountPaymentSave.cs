using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountPaymentSave
    {
        public DateTime PaymentDate { get; set; }

        public string Communication { get; set; }

        public Guid JournalId { get; set; }

        public string PartnerType { get; set; }

        public decimal Amount { get; set; }

        public string PaymentType { get; set; }

        public Guid? PartnerId { get; set; }

        public IEnumerable<Guid> InvoiceIds { get; set; } = new List<Guid>();
    }
}
