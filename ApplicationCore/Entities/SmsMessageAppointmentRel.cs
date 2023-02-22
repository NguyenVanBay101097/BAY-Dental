using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessageAppointmentRel
    {
        public Guid SmsMessageId { get; set; }
        public SmsMessage SmsMessage { get; set; }

        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}
