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
        public EpplusDisplay(string displayName, int sequence = 0)
        {
            DisplayName = displayName;
            Sequence = sequence;
        }
        public string DisplayName { get; set; }
        public int Sequence { get; set; }
    }
}
