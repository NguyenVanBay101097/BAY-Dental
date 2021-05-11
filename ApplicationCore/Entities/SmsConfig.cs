using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsConfig : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public bool IsBirthdayAutomation { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public Guid? TemplateId { get; set; }
        public SmsTemplate Template { get; set; }
    }
}
