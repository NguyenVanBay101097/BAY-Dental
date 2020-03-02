using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FacebookUserProfile: BaseEntity
    {
        public string PsId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string PageId { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
