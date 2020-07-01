using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareScenarioSave
    {
        public string Name { get; set; }

        public IEnumerable<TCareCampaignSave> Campaigns { get; set; } = new List<TCareCampaignSave>();
    }
}
