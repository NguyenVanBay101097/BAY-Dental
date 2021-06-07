using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessageSaleOrderRel
    {
        public Guid SmsMessageId { get; set; }
        public SmsMessage SmsMessage { get; set; }

        public Guid SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }
    }
}
