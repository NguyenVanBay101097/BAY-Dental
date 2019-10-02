using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class District: BaseEntity
    {
        public string Name { get; set; }

        public Guid ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}
