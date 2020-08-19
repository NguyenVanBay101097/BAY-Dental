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
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public double HourFrom { get; set; }
        public double HourTo { get; set; }
        public Guid CalendarId { get; set; }
        public ResourceCalendarBasic Calendar { get; set; }
        public string DayPeriod { get; set; }
        public int Sequence { get; set; }
    }
}
