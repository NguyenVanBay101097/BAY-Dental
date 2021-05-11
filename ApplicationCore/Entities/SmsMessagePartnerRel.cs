using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessagePartnerRel
    {
        public SmsMessagePartner SmsMessagePartner { get; set; }
        public Guid SmsMessagePartnerId { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
