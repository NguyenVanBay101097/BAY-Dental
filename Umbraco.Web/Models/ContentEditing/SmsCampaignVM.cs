using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsCampaignBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LimitMessage { get; set; }
        public DateTime? DateEnd { get; set; }
        public int TotalMessage { get; set; }
        public int TotalSuccessfulMessages { get; set; }
        public int TotalFailedMessages { get; set; }
        public int TotalWaitedMessages { get; set; }
        public DateTime? DateStart { get; set; }
        public string TypeDate { get; set; }
        public string State { get; set; }
        public string DefaultType { get; set; }
    }

    public class SmsCampaignPaged
    {
        public SmsCampaignPaged()
        {
            Combobox = false;
            Limit = 20;
            Offset = 0;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public string State { get; set; }
        public bool? Combobox { get; set; }
    }

    public class SmsCampaignSave
    {
        public string Name { get; set; }
        public int LimitMessage { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateStart { get; set; }
        public string TypeDate { get; set; }
        public string State { get; set; }
    }
}
