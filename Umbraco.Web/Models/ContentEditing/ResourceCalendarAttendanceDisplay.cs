using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResourceCalendarAttendanceDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DayOfWeek { get; set; }
        public double HourFrom { get; set; }
        public double HourTo { get; set; }
        public string DayPeriod { get; set; }
    }
}
