using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerMapPSIDFacebookPage: BaseEntity
    {
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public string PageId { get; set; }

        public string PSId { get; set; }
    }
}
