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

        /// <summary>
        /// 3_global: Áp dụng tất cả dịch vụ
        /// 2_product_category: Áp dụng trên 1 nhóm dịch vụ cụ thể
        /// 0_product_variant: Áp dụng trên 1 dịch vụ cụ thể
        /// 
        /// 4_product_combo : Áp dụng trên combo dịch vụ
        /// </summary>
        public string AppliedOn { get; set; }

        public int MinQuantity { get; set; }

        public int Sequence { get; set; }

        /// <summary>
        /// list_price: Giá bán
        /// </summary>
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

        /// <summary>
        /// fixed: Giá cố định
        /// percentage: Giảm phần trăm
        /// fixed_amount: Giảm tiền
        /// </summary>
        public string ComputePrice { get; set; }

        public decimal? FixedPrice { get; set; }

        public decimal? PercentPrice { get; set; }

        public decimal? FixedAmountPrice { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? PartnerCategId { get; set; }
        public PartnerCategory PartnerCateg { get; set; }

        public Guid? CardTypeId { get; set; }
        public CardType CardType { get; set; }
    }
}
