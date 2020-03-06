using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookPageBasic
    {
        public Guid id { get; set; }     
        public string PageId { get; set; }

        public string PageName { get; set; }
        public string PageAccesstoken { get; set; }
    }
}
