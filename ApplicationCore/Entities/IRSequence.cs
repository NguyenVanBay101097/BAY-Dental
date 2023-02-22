using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRSequence : BaseEntity
    {
        public IRSequence()
        {
            Active = true;
            NumberIncrement = 1;
            NumberNext = 1;
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int NumberNext { get; set; }

        public string Implementation { get; set; }

        public int Padding { get; set; }

        public int NumberIncrement { get; set; }

        public string Prefix { get; set; }

        public bool Active { get; set; }

        public string Suffix { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
