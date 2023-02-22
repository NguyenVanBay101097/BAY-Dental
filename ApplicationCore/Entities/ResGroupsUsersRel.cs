using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResGroupsUsersRel
    {
        public Guid GroupId { get; set; }
        public ResGroup Group { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
