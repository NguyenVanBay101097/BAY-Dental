using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessageSaleOrderLineRel
    {
        public Guid SmsMessageId { get; set; }
        public SmsMessage SmsMessage { get; set; }

        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }
    }
}
