using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResGroup: BaseEntity
    {
        public string Name { get; set; }

        public ICollection<ResGroupsUsersRel> ResGroupsUsersRels { get; set; }

        public ICollection<IRModelAccess> ModelAccesses { get; set; } = new List<IRModelAccess>();
    }
}
