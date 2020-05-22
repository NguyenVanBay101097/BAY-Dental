using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareRuleSave
    {
        public string Type { get; set; }

        public Guid CampaignId { get; set; }

        public IEnumerable<TCarePropertySave> Properties { get; set; } = new List<TCarePropertySave>();
    }
}
