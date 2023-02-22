﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductLaboDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? LaboPrice { get; set; }
        public ProductCategoryBasic Categ { get; set; }
    }
}
