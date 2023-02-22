using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// User account
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Active = true;
        }

        public string Name { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public bool Active { get; set; }

        public bool IsUserRoot { get; set; }

        public ICollection<ResGroupsUsersRel> ResGroupsUsersRels { get; set; } = new List<ResGroupsUsersRel>();

        public ICollection<ResCompanyUsersRel> ResCompanyUsersRels { get; set; } = new List<ResCompanyUsersRel>();

        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();

        public Guid? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }
    }
}
