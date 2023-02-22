using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerPartnerCategoryRel
    {
        public Guid CategoryId { get; set; }
        public PartnerCategory Category { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
