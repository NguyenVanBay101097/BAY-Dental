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

    public class PartnerForTCarePaged
    {
        public int? BirthDay { get; set; }
        public Guid? PartnerCategoryId { get; set; }
        public string Op { get; set; }
        public int? BirthMonth { get; set; }
        public bool? Customer { get; set; }
    }
}
