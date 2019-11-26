using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MailMessageFormat
    {
        public Guid Id { get; set; }

        public string Body { get; set; }

        public DateTime? Date { get; set; }

        public string AuthorName { get; set; }

        public string MessageType { get; set; }

        public string Subject { get; set; }

        public string Model { get; set; }

        public Guid? ResId { get; set; }
    }
}
