using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class UserChangeCurrentFacebookPage
    {
        public FacebookPageBasic CurrentPage { get; set; }
        public IEnumerable<FacebookPageBasic> Pages { get; set; } = new List<FacebookPageBasic>();
    }
}
