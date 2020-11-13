﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareCampaignSave
    {
        public string Name { get; set; }

        public string GraphXml { get; set; }

        public Guid? TagId { get; set; }

        public string SheduleStartType { get; set; }
        public decimal SheduleStartNumber { get; set; }
    }
}
