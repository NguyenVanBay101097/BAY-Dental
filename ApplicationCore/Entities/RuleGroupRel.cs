using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class RuleGroupRel
    {
        public Guid RuleId { get; set; }
        public IRRule Rule { get; set; }

        public Guid GroupId { get; set; }
        public ResGroup Group { get; set; }
    }
}
