using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookConnectDisplay
    {
        public string FbUserId { get; set; }
        public string FbUserName { get; set; }
        public string FbUserAccessToken { get; set; }
        public IEnumerable<FacebookConnectPageDisplay> Pages { get; set; } = new List<FacebookConnectPageDisplay>();
    }
}
