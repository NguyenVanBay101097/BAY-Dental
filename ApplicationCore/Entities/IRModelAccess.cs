using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRModelAccess: BaseEntity
    {
        public IRModelAccess()
        {
            Active = true;
            PermRead = true;
            PermCreate = true;
            PermWrite = true;
            PermUnlink = true;
        }

        public string Name { get; set; }

        public bool Active { get; set; }

        public bool PermRead { get; set; }

        public bool PermWrite { get; set; }

        public bool PermCreate { get; set; }

        public bool PermUnlink { get; set; }

        public Guid ModelId { get; set; }

        public IRModel Model { get; set; }

        public Guid? GroupId { get; set; }

        public ResGroup Group { get; set; }
    }
}
