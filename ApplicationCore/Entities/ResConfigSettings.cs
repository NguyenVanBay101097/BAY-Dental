using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResConfigSettings: BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        [DbColumn("implied_group", "sale.group_discount_per_so_line")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupDiscountPerSOLine { get; set; }

        [DbColumn("implied_group", "sale.group_sale_coupon_promotion")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupSaleCouponPromotion { get; set; }

        [DbColumn("implied_group", "product.group_uom")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupUoM { get; set; }

        [DbColumn("implied_group", "sale.group_loyalty_card")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupLoyaltyCard { get; set; }

        [DbColumn("config_parameter", "loyalty.point_exchange_rate")]
        [DbColumn("field_type", "decimal")]
        public decimal? LoyaltyPointExchangeRate { get; set; }

        [DbColumn("implied_group", "base.group_multi_company")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupMultiCompany { get; set; }

        /// <summary>
        /// Share product to all companies
        /// </summary>
        public bool? CompanyShareProduct { get; set; }

        public bool? CompanySharePartner { get; set; }

        /// <summary>
        /// Giá bán của product sẽ riêng theo từng chi nhánh
        /// </summary>
        [DbColumn("config_parameter", "product.listprice_restrict_company")]
        [DbColumn("field_type", "boolean")]
        public bool? ProductListpriceRestrictCompany { get; set; }

        [DbColumn("implied_group", "sale.group_service_card")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupServiceCard { get; set; }
    }
}
