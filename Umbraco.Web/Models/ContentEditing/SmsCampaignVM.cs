using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsCampaignBasic
    {
        public string Name { get; set; }
    }

    public class SmsCampaignPaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }

    public class SmsCampaignSave
    {
        public string Name { get; set; }
    }
}
