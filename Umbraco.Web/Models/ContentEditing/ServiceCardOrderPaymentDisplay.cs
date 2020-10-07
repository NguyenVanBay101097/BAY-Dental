using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderPaymentDisplay
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        //ngay thanh toan
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }
    }
}
