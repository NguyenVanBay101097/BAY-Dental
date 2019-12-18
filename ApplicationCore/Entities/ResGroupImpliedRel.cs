using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResGroupImpliedRel
    {
        public Guid GId { get; set; }
        public ResGroup G { get; set; }

        public Guid HId { get; set; }
        public ResGroup H { get; set; }
    }
}
