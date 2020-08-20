using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Thời gian làm việc
    /// </summary>
    public class ResourceCalendarAttendance: BaseEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// Day of Week, default='0'
        /// ('1', 'Monday'),
        /// ('2', 'Tuesday'),
        /// ('3', 'Wednesday'),
        /// ('4', 'Thursday'),
        /// ('5', 'Friday'),
        /// ('6', 'Saturday'),
        /// ('0', 'Sunday')
        /// </summary>
        public string DayOfWeek { get; set; }

        /// <summary>
        /// Starting Date
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// End Date
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Sap xep
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Work from
        /// </summary>
        public double HourFrom { get; set; }

        /// <summary>
        /// Work to
        /// </summary>
        public double HourTo { get; set; }

        public Guid? CalendarId { get; set; }
        public ResourceCalendar Calendar { get; set; }

        /// <summary>
        /// default='morning'
        /// ('morning', 'Morning'), ('afternoon', 'Afternoon')
        /// </summary>
        public string DayPeriod { get; set; }
    }
}
