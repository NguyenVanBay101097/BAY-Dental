using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsCareAfterOrderAutomationConfig : BaseEntity
    {
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }

        public DateTime? ScheduleTime { get; set; }

        /// <summary>
        /// Không sử dụng
        /// </summary>
        public string Body { get; set; }

        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }

        public bool Active { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }

        public string ApplyOn { get; set; }

        public ICollection<SmsConfigProductCategoryRel> SmsConfigProductCategoryRels { get; set; } = new List<SmsConfigProductCategoryRel>();
        public ICollection<SmsConfigProductRel> SmsConfigProductRels { get; set; } = new List<SmsConfigProductRel>();

        public DateTime? LastCron { get; set; }
    }
}
