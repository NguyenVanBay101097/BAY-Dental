using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookConfigSave
    {
        public string FbAccountName { get; set; }

        public string FbAccountUserId { get; set; }

        public string UserAccessToken { get; set; }

        public ICollection<FacebookConfigPageSave> ConfigPages { get; set; }
    }
}
