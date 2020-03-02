using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookConfig: BaseEntity
    {
        public string FbAccountName { get; set; }

        public string FbAccountUserId { get; set; }

        public string UserAccessToken { get; set; }

        public ICollection<FacebookConfigPage> ConfigPages { get; set; } = new List<FacebookConfigPage>();
    }
}
