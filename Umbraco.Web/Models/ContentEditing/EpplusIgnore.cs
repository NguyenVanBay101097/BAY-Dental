using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EpplusIgnore : Attribute
    {
    }

    public class EpplusDisplay : Attribute
    {
        public EpplusDisplay(string displayName)
        {
            DisplayName = displayName;
        }
        public string DisplayName { get; set; }
    }
}
