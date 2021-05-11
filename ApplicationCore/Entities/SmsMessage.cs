using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessage : BaseEntity
    {
        public string Name { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }
        public DateTime? Date { get; set; }
        public Guid?  SmsTemplateId { get; set; }
        public SmsTemplate SmsTemplate { get; set; }
        public string State { get; set; }
        public Guid SmsMessagePartnerId { get; set; }
        public SmsMessagePartner SmsMessagePartner { get; set; }
    }
}
