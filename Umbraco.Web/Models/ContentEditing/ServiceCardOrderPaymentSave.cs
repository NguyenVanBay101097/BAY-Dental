using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderPaymentSave
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public Guid JournalId { get; set; }
    }
}
