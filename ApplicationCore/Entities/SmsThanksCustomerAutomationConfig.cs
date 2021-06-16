using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsThanksCustomerAutomationConfig : BaseEntity
    {
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }

        public string Body { get; set; }

        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }

        public bool Active { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }

        public DateTime? LastCron { get; set; }
    }
}
