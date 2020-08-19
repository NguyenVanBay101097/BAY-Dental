﻿using System;
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
        /// ('0', 'Monday'),
        /// ('1', 'Tuesday'),
        /// ('2', 'Wednesday'),
        /// ('3', 'Thursday'),
        /// ('4', 'Friday'),
        /// ('5', 'Saturday'),
        /// ('6', 'Sunday')
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
