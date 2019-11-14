using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResCompanyUsersRel
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
