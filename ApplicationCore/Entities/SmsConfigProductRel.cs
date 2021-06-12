using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsConfigProductRel
    {
        public Guid SmsConfigId { get; set; }
        public SmsCareAfterOrderAutomationConfig SmsConfig { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
