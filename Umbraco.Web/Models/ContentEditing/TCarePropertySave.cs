using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCarePropertySave
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public object Value { get; set; }
    }
}
