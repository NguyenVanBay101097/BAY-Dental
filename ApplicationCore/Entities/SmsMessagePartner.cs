using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessagePartner : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<SmsMessagePartnerRel> SmsMessagePartnerRels { get; set; } = new List<SmsMessagePartnerRel>();
    }
}
