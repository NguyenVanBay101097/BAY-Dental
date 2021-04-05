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

        [DbColumn("implied_group", "base.group_multi_company")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupMultiCompany { get; set; }

        [DbColumn("implied_group", "product.group_uom")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupUoM { get; set; }

        /// <summary>
        /// Share product to all companies
        /// </summary>
        public bool? CompanyShareProduct { get; set; }

        /// <summary>
        /// Giá bán của product sẽ riêng theo từng chi nhánh
        /// </summary>
        [DbColumn("config_parameter", "product.listprice_restrict_company")]
        [DbColumn("field_type", "boolean")]
        public bool? ProductListpriceRestrictCompany { get; set; }

        public bool? CompanySharePartner { get; set; }

        [DbColumn("implied_group", "sale.group_service_card")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupServiceCard { get; set; }

        [DbColumn("implied_group", "tcare.group_tcare")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupTCare { get; set; }

        /// <summary>
        /// Chạy tất cả kịch bản mỗi ngày vào lúc
        /// </summary>
        [DbColumn("config_parameter", "tcare.run_at")]
        [DbColumn("field_type", "datetime")]
        public DateTime? TCareRunAt { get; set; }
        /// <summary>
        /// cấu hình cho bán thuốc
        /// </summary>
        [DbColumn("implied_group", "medicineOrder.group_medicine")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupMedicine { get; set; }

        /// <summary>
        /// cấu hình cho Survey
        /// </summary>
        [DbColumn("implied_group", "survey.group_survey")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupSurvey { get; set; }

    }
}
