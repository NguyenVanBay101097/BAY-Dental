using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class IRRule : BaseEntity
    {
        public IRRule()
        {
            Global = true;
            Active = true;
            PermCreate = true;
            PermWrite = true;
            PermUnlink = true;
            PermRead = true;
        }
        public string Name { get; set; }

        public bool Global { get; set; }

        public bool Active { get; set; }

        public bool PermUnlink { get; set; }

        public bool PermWrite { get; set; }

        public bool PermRead { get; set; }

        public bool PermCreate { get; set; }

        public Guid ModelId { get; set; }
        public IRModel Model { get; set; }

        public ICollection<RuleGroupRel> RuleGroupRels { get; set; } = new List<RuleGroupRel>();

        public string Code { get; set; }
    }
}
