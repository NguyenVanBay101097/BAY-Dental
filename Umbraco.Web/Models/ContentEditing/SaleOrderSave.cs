﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderSave
    {
        public Guid Id { get; set; }

        public DateTime DateOrder { get; set; }

        public Guid PartnerId { get; set; }

        public string Note { get; set; }

        public IEnumerable<SaleOrderLineSave> OrderLines { get; set; } = new List<SaleOrderLineSave>();

        public Guid CompanyId { get; set; }

        public string UserId { get; set; }

        public Guid? PricelistId { get; set; }

        public bool? IsQuotation { get; set; }
    }
}
