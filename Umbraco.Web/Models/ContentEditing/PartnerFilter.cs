using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerFilter
    {
        public string Search { get; set; }

        public bool? Customer { get; set; }

        public bool? Supplier { get; set; }

        public bool? Employee { get; set; }
    }
}
