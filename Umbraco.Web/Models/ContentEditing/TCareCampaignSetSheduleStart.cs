using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareCampaignSetSheduleStart
    {
        public Guid Id { get; set; }

        public string ScheduleStartType { get; set; }
        public decimal ScheduleStartNumber { get; set; }
    }
}
