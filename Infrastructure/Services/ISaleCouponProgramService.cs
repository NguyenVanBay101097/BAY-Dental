using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleCouponProgramService : IBaseService<SaleCouponProgram>
    {
        Task Apply(SaleCouponProgram rule, SaleOrder order, decimal total_amount, decimal total_qty, SaleCoupon coupon = null);
        Task<PagedResult2<SaleCouponProgramBasic>> GetPagedResultAsync(SaleCouponProgramPaged val);
        Task<IEnumerable<SaleCouponProgramBasic>> GetPromotionBySaleOrder(Guid? partnerId);
        Task<IEnumerable<SaleCouponProgramBasic>> GetPromotionBySaleOrderLine(Guid productId, Guid? partnerId);
        Task<IEnumerable<SaleCouponProgramDisplay>> GetPromotionByFastSaleOrder();
        Task<decimal> GetAmountTotal(Guid id);
        Task<SaleCouponProgram> CreateProgram(SaleCouponProgramSave val);
        Task UpdateProgram(Guid id, SaleCouponProgramSave val);
        Task<SaleCouponProgramDisplay> GetDisplay(Guid id);
        Task GenerateCoupons(SaleCouponProgramGenerateCoupons val);
        Task Unlink(IEnumerable<Guid> ids);
        Task ToggleActive(IEnumerable<Guid> ids);
        bool _IsGlobalDiscountProgram(SaleCouponProgram self);
        IEnumerable<SaleCouponProgram> _FilterProgramsFromCommonRules(IEnumerable<SaleCouponProgram> self, SaleOrder order, bool next_order = false);
        Task<CheckPromoCodeMessage> _CheckPromoCode(SaleCouponProgram self, SaleOrder order, string coupon_code);
        Task<CheckPromoCodeMessage> _CheckQuotationPromoCode(SaleCouponProgram self, Quotation quotation, string coupon_code);
        CheckPromoCodeMessage _CheckPromotion(SaleCouponProgram self, SaleOrder order);

        CheckPromoCodeMessage _CheckQuotationPromotion(SaleCouponProgram self, Quotation quotation);
        Task<CheckPromoCodeMessage> _CheckPromotionApplySaleLine(SaleCouponProgram self, SaleOrderLine line);
        CheckPromoCodeMessage _CheckPromotionApplyQuotationLine(SaleCouponProgram self, QuotationLine line);
        IEnumerable<SaleCouponProgram> _FilterOnMinimumAmount(IEnumerable<SaleCouponProgram> self, SaleOrder order);
        IEnumerable<SaleCouponProgram> _FilterOnMinimumAmount(SaleCouponProgram self, SaleOrder order);
        IEnumerable<SaleCouponProgram> _KeepOnlyMostInterestingAutoAppliedGlobalDiscountProgram(IEnumerable<SaleCouponProgram> self);
        IEnumerable<SaleCouponProgram> _FilterPromoProgramsWithCode(IEnumerable<SaleCouponProgram> self, string promo_code);
        Task ActionArchive(IEnumerable<Guid> ids);
        Task ActionUnArchive(IEnumerable<Guid> ids);

        //Task<SaleCouponProgramResponse> GetPromotionDisplayUsageCode(string code, Guid? productId);
        Task<PagedResult2<SaleCouponProgramGetListPagedResponse>> GetListPaged(SaleCouponProgramGetListPagedRequest val);
    }
}
