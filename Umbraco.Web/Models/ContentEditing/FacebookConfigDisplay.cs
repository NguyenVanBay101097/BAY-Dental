using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookConfigDisplay
    {
        public Guid Id { get; set; }

        public string FbAccountName { get; set; }

        public string FbAccountUserId { get; set; }

        public string UserAccessToken { get; set; }

        public IEnumerable<FacebookConfigPageDisplay> ConfigPages { get; set; } = new List<FacebookConfigPageDisplay>();
    }
}
