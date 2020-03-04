using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookConfigPageDisplay
    {
        public Guid Id { get; set; }

        public string PageName { get; set; }

        public string PageId { get; set; }

        public string PageAccessToken { get; set; }
    }
}
