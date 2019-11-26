using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MailNotification: BaseEntity
    {
        public MailNotification()
        {
        }

        public Guid MailMessageId { get; set; }
        public MailMessage MailMessage { get; set; }

        public Guid ResPartnerId { get; set; }
        public Partner ResPartner { get; set; }

        public bool IsRead { get; set; }
    }
}
