using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Pricelist Rule
    /// </summary>
    public class ProductPricelistItem: BaseEntity
    {
        public ProductPricelistItem()
        {
            Base = "list_price";
            MinQuantity = 0;
            Sequence = 5;
            PriceDiscount = 0;
            AppliedOn = "3_global";
            ComputePrice = "fixed";
        }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? CategId { get; set; }
        public ProductCategory Categ { get; set; }

        public string AppliedOn { get; set; }

        public int MinQuantity { get; set; }

        public int Sequence { get; set; }

        public string Base { get; set; }

        public Guid? PriceListId { get; set; }
        public ProductPricelist PriceList { get; set; }

        /// <summary>
        /// Specify the fixed amount to add or substract(if negative) to the amount calculated with the discount.
        /// </summary>
        public decimal? PriceSurcharge { get; set; }

        public decimal? PriceDiscount { get; set; }

        public decimal? PriceRound { get; set; }

        public decimal? PriceMinMargin { get; set; }

        public decimal? PriceMaxMargin { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string ComputePrice { get; set; }

        public decimal? FixedPrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
