using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductPricelistItemCreate
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }

        public string ComputePrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public decimal? FixedAmountPrice { get; set; }
    }

    public class ProductPricelistItemDisplay
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public string ComputePrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public decimal? FixedAmountPrice { get; set; }
    }
}
