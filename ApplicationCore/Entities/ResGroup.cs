using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResGroup: BaseEntity
    {
        public string Name { get; set; }

        public ICollection<ResGroupsUsersRel> ResGroupsUsersRels { get; set; } = new List<ResGroupsUsersRel>();

        public ICollection<IRModelAccess> ModelAccesses { get; set; } = new List<IRModelAccess>();

        public ICollection<RuleGroupRel> RuleGroupRels { get; set; } = new List<RuleGroupRel>();

        public ICollection<ResGroupImpliedRel> ImpliedRels { get; set; } = new List<ResGroupImpliedRel>();

        public Guid? CategoryId { get; set; }
        public IrModuleCategory Category { get; set; }
    }
}
