using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookMassMessaging: BaseEntity
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public string State { get; set; }

        public Guid? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }
    }
}
