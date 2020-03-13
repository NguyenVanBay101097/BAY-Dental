using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookUserProfile: BaseEntity
    {
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string PSID { get; set; }

        public Guid FbPageId { get; set; }
        public FacebookPage FbPage { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
