using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResourceCalendarSave
    {
        public string Name { get; set; }
        public decimal? HoursPerDay { get; set; }
        //public IEnumerable<ResourceCalendarAttendanceSave> Attendances { get; set; }
    }
    public class ResourceCalendarDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? HoursPerDay { get; set; }
        public IEnumerable<ResourceCalendarAttendanceDisplay> Attendances { get; set; }
    }
}
