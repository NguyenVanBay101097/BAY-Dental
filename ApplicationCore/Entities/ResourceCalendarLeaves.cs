using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResourceCalendarLeaves: BaseEntity
    {
        /// <summary>
        /// Reason
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// related='calendar_id.company_id'
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? CalendarId { get; set; }
        public ResourceCalendar Calendar { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
