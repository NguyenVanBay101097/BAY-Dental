using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRModelData: BaseEntity
    {
        public string Name { get; set; }

        public string ResId { get; set; }

        public string Model { get; set; }

        public string Module { get; set; }
    }
}
