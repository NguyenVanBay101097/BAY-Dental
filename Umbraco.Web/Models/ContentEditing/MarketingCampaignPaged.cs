﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MarketingCampaignPaged
    {
        public MarketingCampaignPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public string State { get; set; }
    }
}
