using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsAppointmentAutomationConfig : BaseEntity
    {
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }

        /// <summary>
        /// Thời gian gửi tin trước lịch hẹn
        /// </summary>
        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }
        public string Body { get; set; }
        public bool Active { get; set; }
        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }

        public DateTime? LastCron { get; set; }
    }
}
