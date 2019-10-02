using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ApplicationRoleFunction: BaseEntity
    {
        public string Func { get; set; }

        public string RoleId { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
