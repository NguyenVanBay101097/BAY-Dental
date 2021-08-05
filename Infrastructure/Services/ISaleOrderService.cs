﻿using ApplicationCore.Entities;
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
        Task<SaleOrderDisplay> GetSaleOrderForDisplayAsync(Guid id);
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
        void _ComputeResidual(IEnumerable<SaleOrder> self);

        Task _ComputeAmountPromotionToOrder(IEnumerable<Guid> ids);

        //Task ApplyCoupon(SaleOrderApplyCoupon val);
        Task<SaleCouponProgramResponse> ApplyCoupon(ApplyPromotionUsageCode val);
        Task ApplyPromotion(Guid id);

        Task ApplyPromotionOnOrder(ApplyPromotionRequest val);

        Task ApplyDiscountOnOrder(ApplyDiscountViewModel val);

        Task<SaleOrder> ActionConvertToOrder(Guid id);
        Task<IEnumerable<AccountMove>> ActionInvoiceCreateV2(Guid id);
        bool _IsGlobalDiscountAlreadyApplied(SaleOrder self);
        bool _IsRewardInOrderLines(SaleOrder self, SaleCouponProgram program);
        IEnumerable<SaleOrderLine> _GetRewardLines(SaleOrder self);
        Task<IEnumerable<SaleCouponProgram>> _GetApplicablePrograms(SaleOrder self);
        //Task RecomputeCouponLines(IEnumerable<Guid> ids);
        Task<IEnumerable<PaymentInfoContent>> _GetPaymentInfoJson(Guid id);
        Task RecomputeResidual(IEnumerable<Guid> ids);
        Task<bool> CheckHasPromotionCanApply(Guid id);
        Task<IEnumerable<AccountMoveBasic>> GetInvoicesBasic(Guid id);
        Task ApplyServiceCards(SaleOrderApplyServiceCards val);
        Task<IEnumerable<SaleOrderLineDisplay>> GetServiceBySaleOrderId(Guid id);
        Task<IEnumerable<DotKhamDisplay>> GetTreatmentBySaleOrderId(Guid id);
        Task<IEnumerable<LaboOrderDisplay>> GetLaboBySaleOrderId(Guid id);

        Task<RegisterSaleOrderPayment> GetSaleOrderPaymentBySaleOrderId(Guid id);
        Task ApplyDiscountDefault(ApplyDiscountViewModel val);
        void _AmountAll(SaleOrder order);
        void _AmountAll(IEnumerable<SaleOrder> self);

        //Task CancelSaleOrderLine(ActionCancelSaleOrderLineViewModel val);

        //Task<SaleOrderDisplay> GetDisplayAsync(Guid id);

        Task<IEnumerable<SaleOrderLineDisplay>> GetSaleOrderLineBySaleOrder(Guid id);

        Task<SaleOrder> CreateOrderAsync(SaleOrderSave val);

        Task ActionUnlock(IEnumerable<Guid> ids);

        void _GetInvoiced(IEnumerable<SaleOrder> orders);

        Task<SaleOrderBasic> CreateFastSaleOrder(FastSaleOrderSave val);
        Task<IEnumerable<SaleOrderLineBasicViewModel>> GetDotKhamStepByOrderLine(Guid key);
        Task<IEnumerable<DotKhamDisplayVm>> _GetListDotkhamInfo(Guid id);
        Task<PagedResult2<SaleOrderToSurvey>> GetToSurveyPagedAsync(SaleOrderToSurveyFilter val);

        Task<IEnumerable<SaleOrderLineForProductRequest>> GetLineForProductRequest(Guid id);


        Task<List<SearchAllViewModel>> SearchAll(SaleOrderPaged val);
        Task<PagedResult2<SaleOrderSmsBasic>> GetSaleOrderForSms(SaleOrderPaged val);

        Task<PagedResult2<SaleOrderRevenueReport>> GetRevenueReport(SaleOrderRevenueReportPaged val);

        Task<GetRevenueSumTotalRes> GetRevenueSumTotal(GetRevenueSumTotalReq val);

        Task<IEnumerable<SaleOrderManagementExcel>> GetExcel(SaleOrderPaged val);

        Task ComputeToUpdateSaleOrder(SaleOrder order);
        Task<RevenueReportPrintVM<SaleOrderRevenueReport>> GetRevenueReportPrint(SaleOrderRevenueReportPaged val);
    }
}
