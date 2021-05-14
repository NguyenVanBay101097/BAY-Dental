using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsConfigSave
    {
        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public string Type { get; set; }

        public Guid? TemplateId { get; set; }

        public Guid? SmsAccountId { get; set; }

        public DateTime? DateSend { get; set; }
        public int TimeBeforSend { get; set; }

        public string TypeTimeBeforSend { get; set; }

        public string Body { get; set; }
    }

    public class SmsConfigBasic
    {
        public Guid Id { get; set; }

        public bool IsBirthdayAutomation { get; set; }
        public bool IsAppointmentAutomation { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplateBasic Template { get; set; }

        public Guid? SmsAccountId { get; set; }
        public SmsAccountBasic SmsAccount { get; set; }

        public DateTime? DateSend { get; set; }
        public int TimeBeforSend { get; set; }
        public string TypeTimeBeforSend { get; set; }
        public string Body { get; set; }
    }
}
