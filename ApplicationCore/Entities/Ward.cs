using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Ward: BaseEntity
    {
        public string Name { get; set; }
        public Guid DistrictId { get; set; }
        public District District { get; set; }
    }
}
