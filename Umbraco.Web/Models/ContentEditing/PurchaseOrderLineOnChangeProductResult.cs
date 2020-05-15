﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderLineOnChangeProductResult
    {
        public string Name { get; set; }
        public decimal PriceUnit { get; set; }
        public Guid? ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }
    }

    public class PurchaseOrderLineOnChangeUOMResult
    {
        public decimal PriceUnit { get; set; }
    }
}
