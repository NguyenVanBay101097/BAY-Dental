using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IQuotationService : IBaseService<Quotation>
    {
        Task<PagedResult2<QuotationBasic>> GetPagedResultAsync(QuotationPaged val);
        Task<QuotationDisplay> GetDisplay(Guid id);
        Task<QuotationBasic> CreateAsync(QuotationSave val);
        Task UpdateAsync(Guid id, QuotationSave val);
        Task<QuotationDisplay> GetDefault(Guid partnerId);
        Task<SaleOrderSimple> CreateSaleOrderByQuotation(Guid id);

        Task ApplyDiscountOnQuotation(ApplyDiscountViewModel val);

        Task ApplyPromotionOnQuotation(ApplyPromotionRequest val);

        Task<SaleCouponProgramResponse> ApplyPromotionUsageCode(ApplyPromotionUsageCode val);

        Task _ComputeAmountPromotionToQuotation(IEnumerable<Guid> ids);
        Task<QuotationPrintVM> Print(Guid id);
    }
}
