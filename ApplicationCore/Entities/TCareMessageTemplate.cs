using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareMessageTemplate: BaseEntity
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
