using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderApplyCoupon
    {
        public Guid Id { get; set; }
        public string CouponCode { get; set; }
    }

    public class ApplyPromotionRequest
    {
        public Guid Id { get; set; }
        public Guid SaleProgramId { get; set; }
    }
}
