using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MarketingMessageButton: BaseEntity
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Payload { get; set; }
        public Guid MessageId { get; set; }
        public MarketingMessage Message { get; set; }
    }
}
