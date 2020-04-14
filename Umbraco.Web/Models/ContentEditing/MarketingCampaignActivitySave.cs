using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MarketingCampaignActivitySave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// "hours", "days", "weeks", "months"
        /// </summary>
        public string IntervalType { get; set; }

        public int? IntervalNumber { get; set; }

        public string Template { get; set; }

        public string Text { get; set; }

        public string TriggerType { get; set; }

        public ICollection<MarketingCampaignActivitySave> ActivityChilds { get; set; } = new List<MarketingCampaignActivitySave>();
        public ICollection<MarketingMessageButtonSave> Buttons { get; set; } = new List<MarketingMessageButtonSave>();
    }
}
