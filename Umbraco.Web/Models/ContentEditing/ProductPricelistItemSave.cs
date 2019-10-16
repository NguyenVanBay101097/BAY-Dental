using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductPricelistItemSave
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? CategId { get; set; }

        public string AppliedOn { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string ComputePrice { get; set; }

        public decimal? FixedPrice { get; set; }

        public decimal? PercentPrice { get; set; }
    }
}
