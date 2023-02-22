using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AppointmentMailMessageRel
    {
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public Guid MailMessageId { get; set; }
        public MailMessage MailMessage { get; set; }
    }
}
