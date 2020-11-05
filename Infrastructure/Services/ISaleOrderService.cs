using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderService : IBaseService<SaleOrder>
    {
        Task<PagedResult<SaleOrder>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "");
        Task<SaleOrder> GetSaleOrderForDisplayAsync(Guid id);
        Task<SaleOrder> GetSaleOrderWithLines(Guid id);
        Task UpdateOrderAsync(Guid id, SaleOrderSave val);
        Task<SaleOrder> GetSaleOrderByIdAsync(Guid id);
        Task UnlinkSaleOrderAsync(SaleOrder order);

        Task<SaleOrderLineDisplay> DefaultLineGet(SaleOrderLineDefaultGet val);
        Task<SaleOrderDisplay> DefaultGet(SaleOrderDefaultGet val);
        Task<PagedResult2<SaleOrderBasic>> GetPagedResultAsync(SaleOrderPaged val);
        Task ActionConfirm(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);
        Task<SaleOrderPrintVM> GetPrint(Guid id);
        Task ActionDone(IEnumerable<Guid> ids);
        IEnumerable<Guid> DefaultGetInvoice(List<Guid> ids);
        void _ComputeResidual(IEnumerable<AccountInvoice> invoices);
        Task ApplyCoupon(SaleOrderApplyCoupon val);
        Task ApplyPromotion(Guid id);

        Task<SaleOrder> ActionConvertToOrder(Guid id);
        Task<IEnumerable<AccountMove>> ActionInvoiceCreateV2(Guid id);
        bool _IsGlobalDiscountAlreadyApplied(SaleOrder self);
        bool _IsRewardInOrderLines(SaleOrder self, SaleCouponProgram program);
        IEnumerable<SaleOrderLine> _GetRewardLines(SaleOrder self);
        Task<IEnumerable<SaleCouponProgram>> _GetApplicablePrograms(SaleOrder self);
        Task RecomputeCouponLines(IEnumerable<Guid> ids);
        Task<IEnumerable<PaymentInfoContent>> _GetPaymentInfoJson(Guid id);
        Task RecomputeResidual(IEnumerable<Guid> ids);
        Task<bool> CheckHasPromotionCanApply(Guid id);
        Task<IEnumerable<AccountMoveBasic>> GetInvoicesBasic(Guid id);
        Task ApplyServiceCards(SaleOrderApplyServiceCards val);
        Task<IEnumerable<SaleOrderLineDisplay>> GetServiceBySaleOrderId(Guid id);
        Task<IEnumerable<DotKhamDisplay>> GetTreatmentBySaleOrderId(Guid id);
        Task<IEnumerable<LaboOrderDisplay>> GetLaboBySaleOrderId(Guid id);
        Task ApplyDiscountDefault(ApplyDiscountSaleOrderViewModel val);
        void _AmountAll(SaleOrder order);

        //Task CancelSaleOrderLine(ActionCancelSaleOrderLineViewModel val);

        Task<SaleOrderDisplay> GetDisplayAsync(Guid id);

        Task<SaleOrder> CreateOrderAsync(SaleOrderSave val);

        Task ActionUnlock(IEnumerable<Guid> ids);

        void _GetInvoiced(IEnumerable<SaleOrder> orders);
    }
}
