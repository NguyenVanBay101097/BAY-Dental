using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountPaymentFilter
    {
        public Guid? SaleOrderId { get; set; }

        public string PartnerType { get; set; }

        public string State { get; set; }

        public Guid? PartnerId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Guid? JournalId { get; set; }
    }
}
