using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookUserProfileTagRel
    {
        public Guid UserProfileId { get; set; }
        public FacebookUserProfile UserProfile { get; set; }

        public Guid TagId { get; set; }
        public FacebookTag Tag { get; set; }
    }
}
