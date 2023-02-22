using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookMassMessagingDisplay
    {
        public FacebookMassMessagingDisplay()
        {
            State = "draft";
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public string State { get; set; }

        public Guid? FacebookPageId { get; set; }

        public int TotalSent { get; set; }
        public int TotalReceived { get; set; }
        public int TotalOpened { get; set; }

        public string AudienceFilter { get; set; }
    }
}
