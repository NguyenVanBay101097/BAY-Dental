using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResourceCalendarAttendanceDisplay
    {
        public ResourceCalendarAttendanceDisplay()
        {

        }
        public ResourceCalendarAttendanceDisplay ( string name, string dayofWeek, double hourFrom, double hourTo, string dayperiod)
        {
            this.Name = name;
            this.DayOfWeek = dayofWeek;
            this.HourFrom = hourFrom;
            this.HourTo = hourTo;
            this.DayPeriod = dayperiod;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DayOfWeek { get; set; }
        public double HourFrom { get; set; }
        public double HourTo { get; set; }
        public string DayPeriod { get; set; }
    }
}
