using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CreateServiceCardTypeReq
    {
        public string Name { get; set; }
        public string Period { get; set; }

        public int? NbrPeriod { get; set; }
        public IEnumerable<ProductPricelistItemCreate> ProductPricelistItems { get; set; } = new List<ProductPricelistItemCreate>();
    }

    public class CreateServiceCardTypeRes
    {
        public string Name { get; set; }
        public string Period { get; set; }

        public int? NbrPeriod { get; set; }
        public IEnumerable<ProductPricelistItemDisplay> ProductPricelistItems { get; set; } = new List<ProductPricelistItemDisplay>();
    }
}
