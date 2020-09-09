using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResourceCalendarSave
    {
        public string Name { get; set; }
        public decimal? HoursPerDay { get; set; }
        public IList<ResourceCalendarAttendanceSave> Attendances { get; set; } = new List<ResourceCalendarAttendanceSave>();
        public IList<ResourceCalendarLeaveSave> Leaves { get; set; } = new List<ResourceCalendarLeaveSave>();
        public Guid? CompanyId { get; set; }
    }
}
