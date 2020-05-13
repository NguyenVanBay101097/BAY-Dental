﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountRegisterPaymentDisplay
    {
        public AccountRegisterPaymentDisplay()
        {
            PaymentDate = DateTime.Now;
        }

        public Guid Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Communication { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public string PartnerType { get; set; }

        public decimal Amount { get; set; }

        public string PaymentType { get; set; }

        public Guid? PartnerId { get; set; }

        public IEnumerable<Guid> InvoiceIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> SaleOrderIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> ServiceCardOrderIds { get; set; } = new List<Guid>();
    }
}
