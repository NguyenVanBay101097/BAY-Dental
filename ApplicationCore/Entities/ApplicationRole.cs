using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ApplicationRole: IdentityRole
    {
        public ICollection<ApplicationRoleFunction> Functions { get; set; } = new List<ApplicationRoleFunction>();
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
