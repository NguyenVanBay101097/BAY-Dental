using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsConfigSave
    {
        public Guid CompanyId { get; set; }

        public bool IsBirthdayAutomation { get; set; }

        public Guid? BirthdayTemplateId { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public Guid? AppointmentTemplateId { get; set; }
    }

    public class SmsConfigBasic
    {
        public Guid Id { get; set; }
      
        public bool IsBirthdayAutomation { get; set; }

        public Guid? BirthdayTemplateId { get; set; }
        public SmsTemplateBasic BirthdayTemplate { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public Guid? AppointmentTemplateId { get; set; }
        public SmsTemplateBasic AppointmentTemplate { get; set; }
    }
}
