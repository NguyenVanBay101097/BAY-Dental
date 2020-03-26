using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// cấu hình nhắc lịch hẹn
    /// </summary>
    public class FacebookScheduleAppointmentConfig : BaseEntity
    {

        public FacebookScheduleAppointmentConfig()
        {
            ScheduleType = "hours";
            ScheduleNumber = 1;
            AutoScheduleAppoint = false;
        }
        /// Config Shedule Appointment
        /// <summary>
        /// Create Date Appoint - 
        /// "hours", "minutes"
        /// </summary>
        public string ScheduleType { get; set; }

        public int? ScheduleNumber { get; set; }

        public bool AutoScheduleAppoint { get; set; }

        public string ContentMessage { get; set; }

        public string RecurringJobId { get; set; }

        ///Config happy birthday customers

    }
}
