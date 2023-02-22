using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrderPartnerRel: BaseEntity
    {
        public Guid CardOrderId { get; set; }
        public ServiceCardOrder CardOrder { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
