using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookConnect: BaseEntity
    {
        public string FbUserId { get; set; }
        public string FbUserName { get; set; }
        public string FbUserAccessToken { get; set; }
        public ICollection<FacebookConnectPage> Pages { get; set; } = new List<FacebookConnectPage>();
    }
}
