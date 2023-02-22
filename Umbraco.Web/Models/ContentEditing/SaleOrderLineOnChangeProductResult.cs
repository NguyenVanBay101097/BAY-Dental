using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineOnChangeProductResult
    {
        public decimal PriceUnit { get; set; }

        public string Name { get; set; }

        public decimal Discount { get; set; }
    }
}
