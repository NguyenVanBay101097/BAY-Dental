using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderPaymentSave
    {
        public decimal Amount { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public Guid JournalId { get; set; }
    }
}
