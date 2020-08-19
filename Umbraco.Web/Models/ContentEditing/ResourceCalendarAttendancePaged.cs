using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResourceCalendarAttendancePaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public Guid? ResourceCalendarId { get; set; }
    }
}
