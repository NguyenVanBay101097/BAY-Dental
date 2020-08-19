using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResourceCalendarAttendanceSave
    {
        public ResourceCalendarAttendanceSave()
        {
            DayPeriod = "morning";
        }
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
        /// Work from
        /// </summary>
        public double HourFrom { get; set; }

        /// <summary>
        /// Work to
        /// </summary>
        public double HourTo { get; set; }



        public Guid? CalendarId { get; set; }

        /// <summary>
        /// default='morning'
        /// ('morning', 'Morning'), ('afternoon', 'Afternoon')
        /// </summary>
        public string DayPeriod { get; set; }

        public int Sequence { get; set; }
    }
}
