using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResConfigSettingsSave
    {
        public bool? GroupDiscountPerSOLine { get; set; }

        public bool? GroupSaleCouponPromotion { get; set; }

        public bool? GroupLoyaltyCard { get; set; }

        public decimal? LoyaltyPointExchangeRate { get; set; }

        public bool? GroupMultiCompany { get; set; }

        /// <summary>
        /// Share product to all companies
        /// </summary>
        public bool? CompanyShareProduct { get; set; }

        public bool? CompanySharePartner { get; set; }

        public bool? GroupServiceCard { get; set; }
    }
}
