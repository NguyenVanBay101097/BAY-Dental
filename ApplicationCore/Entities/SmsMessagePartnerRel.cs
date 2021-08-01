using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessagePartnerRel
    {
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public Guid SmsMessageId { get; set; }
        public SmsMessage SmsMessage { get; set; }
    }
}
