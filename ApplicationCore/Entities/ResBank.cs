using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResBank : BaseEntity
    {
        public ResBank()
        {
            Active = true;
        }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string BIC { get; set; }
    }
}
