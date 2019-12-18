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
    }
}
