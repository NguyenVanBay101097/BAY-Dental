using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class UserRefreshToken: BaseEntity
    {
        public string Token { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime? Expiration { get; set; }
    }
}
