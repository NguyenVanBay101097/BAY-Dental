using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Điều kiện
    /// </summary>
    public class TCareRule: BaseEntity
    {
        public Guid CampaignId { get; set; }
        public TCareCampaign Campaign { get; set; }

        public string Type { get; set; }

        public ICollection<TCareProperty> Properties { get; set; } = new List<TCareProperty>();
    }
}
