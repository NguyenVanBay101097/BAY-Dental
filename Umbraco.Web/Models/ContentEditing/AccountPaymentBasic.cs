using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountPaymentBasic
    {
        public Guid Id { get; set; }

        public string PartnerName { get; set; }

        public DateTime PaymentDate { get; set; }
        
        public string JournalName { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public string Communication { get; set; }
    }
}
