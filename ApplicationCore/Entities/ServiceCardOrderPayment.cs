using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrderPayment: BaseEntity
    {
        public Guid OrderId { get; set; }
        public ServiceCardOrder Order { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }
    }
}
