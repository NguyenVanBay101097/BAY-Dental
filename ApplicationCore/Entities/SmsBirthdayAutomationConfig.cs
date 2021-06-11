using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsBirthdayAutomationConfig : BaseEntity
    {
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }

        /// <summary>
        /// Thời điểm gửi tin
        /// </summary>
        public DateTime? ScheduleTime { get; set; }

        /// <summary>
        /// Thoi gian trước bao nhiêu ngày sinh nhật
        /// </summary>
        public int DayBeforeSend { get; set; }

        public string Body { get; set; }
        public bool Active { get; set; }
        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }
    }
}
