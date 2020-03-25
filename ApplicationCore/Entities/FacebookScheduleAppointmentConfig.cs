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
        public FacebookScheduleAppointmentConfig(){
            AutoScheduleAppoint = false;
            ScheduleType = "day";
            ScheduleNumber = 1;
        }
        public Guid FBPageId {get;set;}    

        public FacebookPage FacebookPage { get; set; }
        /// <summary>
        /// Create Date Appoint - 
        /// "hours", "days", "weeks", "months"
        /// </summary>
        public string ScheduleType { get; set; }

        public int? ScheduleNumber { get; set; }

        public bool AutoScheduleAppoint { get; set; }

        public string ContentMessage { get; set; }

        public string JobId { get; set; }
    }
}
