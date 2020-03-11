using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerMapPSIDFacebookPageSave
    {
        public Guid PartnerId { get; set; }
        public string PageId { get; set; }

        public string PSId { get; set; }
    }
}
