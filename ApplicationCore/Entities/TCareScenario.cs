using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareScenario : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<TCareCampaign> Campaigns { get; set; } = new List<TCareCampaign>();

    }
}
