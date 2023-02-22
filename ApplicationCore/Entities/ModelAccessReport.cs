using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ModelAccessReport
    {
        public string UserId { get; set; }

        public string Model { get; set; }

        public Guid? GroupId { get; set; }

        public bool Active { get; set; }
        public bool PermRead { get; set; }
        public bool PermWrite { get; set; }
        public bool PermCreate { get; set; }
        public bool PermUnlink { get; set; }
    }
}
