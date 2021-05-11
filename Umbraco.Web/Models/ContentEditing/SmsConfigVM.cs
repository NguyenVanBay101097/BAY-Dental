using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsConfigSave
    {
        public Guid CompanyId { get; set; }

        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public Guid? TemplateId { get; set; }

        public Guid? SmsAccountId { get; set; }

        public DateTime? ThoiDiem { get; set; }
        public int ThoiGian { get; set; }

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

        public DateTime? ThoiDiem { get; set; }
        public int ThoiGian { get; set; }
        public string Body { get; set; }
    }
}
