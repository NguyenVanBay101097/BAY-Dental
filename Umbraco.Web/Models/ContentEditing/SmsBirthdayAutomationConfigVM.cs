using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SmsBirthdayAutomationConfigSave
    {
        public string Name { get; set; }
       
        public bool Active { get; set; }

        public Guid? TemplateId { get; set; }

        public Guid? SmsAccountId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime? ScheduleTime { get; set; }
        public int DayBeforeSend { get; set; }

        public string Body { get; set; }

        public Guid? SmsCampaignId { get; set; }

    }

    public class SmsBirthdayAutomationConfigDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplateBasic Template { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccountBasic SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaignSimple SmsCampaign { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public int DayBeforeSend { get; set; }
        public string Body { get; set; }
    }
}
