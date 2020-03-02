using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookConversation: BaseEntity
    {
        /// <summary>
        /// facebook_object_id=t_2757672387651911
        /// </summary>
        public string FacebookObjectId { get; set; }

        public string FacebookPageId { get; set; }

        public string Snippet { get; set; }

        public int MessageCount { get; set; }

        public int UnreadCount { get; set; }

        public Guid? UserId { get; set; }
        public FacebookUserProfile User { get; set; }
    }
}
