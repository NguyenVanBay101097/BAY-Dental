using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsConfig : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }

        public DateTime? DateSend { get; set; }

        public string Body { get; set; }

        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }

        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }
        public bool IsCareAfterOrderAutomation { get; set; }
        public bool IsThanksCustomerAutomation { get; set; }

        public string Type { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }
    }
}
