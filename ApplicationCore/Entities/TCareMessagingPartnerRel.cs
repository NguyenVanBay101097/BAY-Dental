using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareMessagingPartnerRel
    {
        public Guid MessagingId { get; set; }
        public TCareMessaging Messaging { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
