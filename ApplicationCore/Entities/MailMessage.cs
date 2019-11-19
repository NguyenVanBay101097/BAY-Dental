using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// system notification
    /// </summary>
    public class MailMessage: BaseEntity
    {
        public MailMessage()
        {
            Date = DateTime.Now;
            MessageType = "email";
        }

        public string Subject { get; set; }

        public DateTime? Date { get; set; }

        /// <summary>
        /// Contents, html
        /// </summary>
        public string Body { get; set; }
        //Task: 'task.assigned'
        //Team Assign: 'team.assign'
        
        public string Model { get; set; }
        //8
        public Guid? ResId { get; set; }
        //name of object
        public string RecordName { get; set; }

        /// <summary>
        /// Message type: email for email message, notification for system
        /// message, comment for other messages such as user replies
        /// ('email', 'Email'),
        /// ('follow', 'Follow task')
        /// ('notification.team', 'Add new user in a team')
        /// ('comment', 'Comment'),
        /// ('notification', 'System notification')
        /// </summary>
        public string MessageType { get; set; }

        public string EmailFrom { get; set; }

        public Guid? AuthorId { get; set; }
        public Partner Author { get; set; }

        public ICollection<MailTrackingValue> TrackingValues { get; set; } = new List<MailTrackingValue>();
    }
}
