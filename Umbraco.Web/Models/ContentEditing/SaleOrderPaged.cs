﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPaged
    {
        public SaleOrderPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }
    }
}
