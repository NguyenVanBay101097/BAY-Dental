﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareCampaignBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string GraphXml { get; set; }

        public string State { get; set; }

        public Guid? TagId { get; set; }

        public Guid TCareScenarioId { get; set; }

        public DateTime? SheduleStart { get; set; }

        public bool Active { get; set; }
    }
}
