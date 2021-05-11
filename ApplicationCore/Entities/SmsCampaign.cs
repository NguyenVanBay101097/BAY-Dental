using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsCampaign : BaseEntity
    {
        public string Name { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
