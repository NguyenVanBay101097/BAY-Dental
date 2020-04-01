using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookMessagingTrace: BaseEntity
    {
        public Guid? MassMessagingId { get; set; }
        public FacebookMassMessaging MassMessaging { get; set; }

        public DateTime? Sent { get; set; }

        public DateTime? Exception { get; set; }

        public DateTime? Delivered { get; set; }

        public DateTime? Opened { get; set; }

        public string MessageId { get; set; }

        public string State { get; set; }

        public Guid? UserProfileId { get; set; }
        public FacebookUserProfile UserProfile { get; set; }
    }
}
