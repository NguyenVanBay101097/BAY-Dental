using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MailMessageResPartnerRel
    {
        public Guid MailMessageId { get; set; }
        public MailMessage MailMessage { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
