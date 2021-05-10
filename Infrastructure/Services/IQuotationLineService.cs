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
        void _ComputeAmountDiscountTotal(IEnumerable<QuotationLine> self);
        void ComputeAmount(IEnumerable<QuotationLine> self);

        decimal _GetRewardValuesDiscountPercentagePerLine(SaleCouponProgram program, QuotationLine line);

        void RecomputePromotionLine(IEnumerable<QuotationLine> self);
    }
}
