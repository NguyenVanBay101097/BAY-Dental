using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPaymentJournalLineBasic
    {
        public Guid Id { get; set; }
        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public decimal Amount { get; set; }
    }

    public class SaleOrderPaymentJournalLineSave
    {
        public Guid Id { get; set; }
        public Guid JournalId { get; set; }

        public decimal Amount { get; set; }
    }

    public class SaleOrderPaymentJournalLineDisplay
    {
        public Guid Id { get; set; }
        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public decimal Amount { get; set; }
    }
}
