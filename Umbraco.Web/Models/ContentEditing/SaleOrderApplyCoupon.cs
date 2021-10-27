using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ApplyPromotionUsageCode
    {
        public Guid Id { get; set; }
        public string CouponCode { get; set; }
    }

    public class ApplyPromotionRequest
    {
        public Guid Id { get; set; }
        public Guid SaleProgramId { get; set; }
    }

    public class ApplyServiceCardCardRequest
    {
        public Guid Id { get; set; }
        public Guid ServiceCardId { get; set; }
    }

    public class ApplyCardCardRequest
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
    }
}
