using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrSalaryRuleCategory : BaseEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public Guid? ParentId { get; set; }
        public HrSalaryRuleCategory Parent { get; set; }
    }
}
