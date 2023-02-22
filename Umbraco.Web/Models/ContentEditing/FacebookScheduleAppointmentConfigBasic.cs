using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookScheduleAppointmentConfigBasic
    {
        public Guid id { get; set; }
        public string ScheduleType { get; set; }

        public int? ScheduleNumber { get; set; }

        public bool AutoScheduleAppoint { get; set; }

        public string ContentMessage { get; set; }

        public string RecurringJobId { get; set; }
    }
}
