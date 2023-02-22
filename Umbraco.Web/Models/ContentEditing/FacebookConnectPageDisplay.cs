using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookConnectPageDisplay
    {
        public Guid Id { get; set; }
        public string PageId { get; set; }
        public string PageName { get; set; }
        public string PageAccessToken { get; set; }
        public string Picture { get; set; }

        public bool Connected { get; set; }
    }
}
