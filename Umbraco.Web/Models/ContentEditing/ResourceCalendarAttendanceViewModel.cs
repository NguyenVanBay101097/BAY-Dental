using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class ResourceCalendarAttendanceSave
    {
        public string Name { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public double HourFrom { get; set; }
        public double HourTo { get; set; }

        public Guid? CalendarId { get; set; }
        public string DayPeriod { get; set; }
    }

    public class ResourceCalendarAttendanceDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public double HourFrom { get; set; }
        public double HourTo { get; set; }

        public Guid? CalendarId { get; set; }
        public string DayPeriod { get; set; }
    }
}
