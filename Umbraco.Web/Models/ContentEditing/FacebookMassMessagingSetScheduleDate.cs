using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookMassMessagingSetScheduleDate
    {
        public Guid MassMessagingId { get; set; }
        public DateTime? ScheduleDate { get; set; }
    }
}
