using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResConfigSettingsDisplay
    {
        [DbColumn("implied_group", "sale.group_discount_per_so_line")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupDiscountPerSOLine { get; set; }

        [DbColumn("implied_group", "sale.group_sale_coupon_promotion")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupSaleCouponPromotion { get; set; }

        [DbColumn("implied_group", "sale.group_loyalty_card")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupLoyaltyCard { get; set; }

        [DbColumn("config_parameter", "loyalty.point_exchange_rate")]
        [DbColumn("field_type", "decimal")]
        public decimal? LoyaltyPointExchangeRate { get; set; }
    }
}
