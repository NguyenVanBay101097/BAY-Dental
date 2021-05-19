using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IQuotationLineService : IBaseService<QuotationLine>
    {
        Task ApplyDiscountOnQuotationLine(ApplyDiscountViewModel val);

        Task<SaleCouponProgramResponse> ApplyPromotionOnQuotationLine(ApplyPromotionRequest val);

        Task<SaleCouponProgramResponse> ApplyPromotionUsageCodeOnQuotationLine(ApplyPromotionUsageCode val);

        void _ComputeAmountDiscountTotal(IEnumerable<QuotationLine> self);
        void ComputeAmount(IEnumerable<QuotationLine> self);

        decimal _GetRewardValuesDiscountPercentagePerLine(SaleCouponProgram program, QuotationLine line);

        void RecomputePromotionLine(IEnumerable<QuotationLine> self);
    }
}
