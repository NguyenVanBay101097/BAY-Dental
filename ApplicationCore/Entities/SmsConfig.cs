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

        public Guid? BirthdayTemplateId { get; set; }
        public TSMSTemplate BirthdayTemplate { get; set; }

        public bool IsAppointmentAutomation { get; set; }

        public Guid? AppointmentTemplateId { get; set; }
        public TSMSTemplate AppointmentTemplate { get; set; }
    }
}
