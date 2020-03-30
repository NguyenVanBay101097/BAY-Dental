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

        public string MessageId { get; set; }

        public string State { get; set; }
    }
}
