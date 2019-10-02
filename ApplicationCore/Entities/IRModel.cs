using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRModel: BaseEntity
    {
        public string Name { get; set; }

        public string Model { get; set; }

        public ICollection<IRModelAccess> ModelAccesses { get; set; } = new List<IRModelAccess>();
    }
}
