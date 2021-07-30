using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsThanksCustomerAutomationConfigSave
    {
        public bool Active { get; set; }

        public Guid? TemplateId { get; set; }

        public Guid? SmsAccountId { get; set; }

        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }

        public Guid? SmsCampaignId { get; set; }
    }

    public class SmsThanksCustomerAutomationConfigDisplay
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


        public int TimeBeforSend { get; set; }
        public string TypeTimeBeforSend { get; set; }
        public string Body { get; set; }
    }
}
