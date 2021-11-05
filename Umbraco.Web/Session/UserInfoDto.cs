using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Session
{
    public class UserInfoDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Avatar { get; set; }

        public Guid CompanyId { get; set; }
    }
}
