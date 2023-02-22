using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SendGridConfig
    {
        public string ApiKey { get; set; }

        public int Port { get; set; }

        public string Host { get; set; }

        public int TimeOut { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string From { get; set; }

        public string TemplateId { get; set; }
    }
}
