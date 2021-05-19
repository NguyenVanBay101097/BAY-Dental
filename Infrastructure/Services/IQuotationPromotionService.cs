using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IQuotationPromotionService : IBaseService<QuotationPromotion>
    {
        Task<PagedResult2<QuotationPromotionBasic>> GetPagedResultAsync(QuotationPromotionPaged val);

        Task RemovePromotion(IEnumerable<Guid> ids);

        QuotationPromotion PreparePromotionToQuotation(Quotation self, SaleCouponProgram program, decimal discountAmount);
        QuotationPromotion PreparePromotionToQuotationLine(QuotationLine self, SaleCouponProgram program, decimal discountAmount);
    }
}
