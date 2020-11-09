using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookUserProfileViewModel
    {
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Avatar { get; set; }

        public string Phone { get; set; }

        public string PSID { get; set; }

        public Guid FbPageId { get; set; }

        public Guid? PartnerId { get; set; }
    }
}
