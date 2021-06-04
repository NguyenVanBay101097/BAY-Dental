using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsConfigSave
    {
        public string Name { get; set; }
        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public bool IsCareAfterOrderAutomation { get; set; }

        public bool IsThanksCustomerAutomation { get; set; }

        public string Type { get; set; }

        public Guid? TemplateId { get; set; }

        public Guid? SmsAccountId { get; set; }

        public DateTime? DateSend { get; set; }
        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }

        public string Body { get; set; }

        public Guid? CampaignId { get; set; }

        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> ProductCategoryIds { get; set; } = new List<Guid>();
    }

    public class SmsConfigPaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public string Type { get; set; }
        public string States { get; set; }
        public bool? IsBirthdayAutomation { get; set; }
        public bool? IsAppointmentAutomation { get; set; }
        public bool? IsCareAfterOrderAutomation { get; set; }
        public bool? IsThanksCustomerAutomation { get; set; }

    }

    public class SmsConfigBasic
    {
        public Guid Id { get; set; }

        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public bool IsCareAfterOrderAutomation { get; set; }

        public bool IsThanksCustomerAutomation { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplateBasic Template { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccountBasic SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaignSimple SmsCampaign { get; set; }

        public DateTime? DateSend { get; set; }
        public int TimeBeforSend { get; set; }
        public string TypeTimeBeforSend { get; set; }
        public string Body { get; set; }
    }

    public class SmsConfigDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public bool IsCareAfterOrderAutomation { get; set; }

        public bool IsThanksCustomerAutomation { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplateBasic Template { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccountBasic SmsAccount { get; set; }

        public Guid? SmsCampaignId { get; set; }
        public SmsCampaignSimple SmsCampaign { get; set; }


        public DateTime? DateSend { get; set; }
        public int TimeBeforSend { get; set; }
        public string TypeTimeBeforSend { get; set; }
        public string Body { get; set; }

        public IEnumerable<ProductSimple> Products { get; set; } = new List<ProductSimple>();
        public IEnumerable<ProductCategorySimple> ProductCategories { get; set; } = new List<ProductCategorySimple>();
    }

    public class SmsConfigGrid
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public DateTime? Date { get; set; }
        public string TypeTimeBeforSend { get; set; }
        public int TimeBeforSend { get; set; }
        public bool IsBirthdayAutomation { get; set; }
        public bool IsAppointmentAutomation { get; set; }
        public bool IsCareAfterOrderAutomation { get; set; }
        public bool IsThanksCustomerAutomation { get; set; }
        public string ProductNames { get; set; }
        public string ProductCategoryNames { get; set; }

    }
}
