using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookMassMessaging: BaseEntity
    {
        public FacebookMassMessaging()
        {
            State = "draft";
        }

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public string State { get; set; }

        public Guid? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }

        public ICollection<FacebookMessagingTrace> Traces { get; set; } = new List<FacebookMessagingTrace>();

        public string JobId { get; set; }

        /// <summary>
        /// Điều kiện lọc những khách hàng sẽ gửi tin nhắn
        /// </summary>
        public string AudienceFilter { get; set; }
    }
}
