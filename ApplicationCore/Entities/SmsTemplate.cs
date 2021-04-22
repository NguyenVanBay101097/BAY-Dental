using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsTemplate : BaseEntity
    {
        public string Name { get; set; }

        public string Body { get; set; }
    }
}
