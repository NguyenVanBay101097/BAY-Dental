using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderLineService : IBaseService<SaleOrderLine>
    {
        void ComputeAmount(IEnumerable<SaleOrderLine> orderLines);

        /// <summary>
        /// Cập nhật 1 số thông tin từ sale order vào sale order line như company, salesman...
        /// </summary>
        /// <param name="orderLines"></param>
        /// <param name="order"></param>
        void UpdateOrderInfo(ICollection<SaleOrderLine> orderLines, SaleOrder order);
        Task<SaleOrderLineOnChangeProductResult> OnChangeProduct(SaleOrderLineOnChangeProduct val);
        void _GetToInvoiceQty(IEnumerable<SaleOrderLine> lines);
        void _GetToInvoiceAmount(IEnumerable<SaleOrderLine> lines);
        void _ComputeInvoiceStatus(IEnumerable<SaleOrderLine> lines);
        void _GetInvoiceQty(IEnumerable<SaleOrderLine> lines);
        void _GetInvoiceAmount(IEnumerable<SaleOrderLine> self);
        AccountInvoiceLine _PrepareInvoiceLine(SaleOrderLine line, decimal qty, AccountAccount account);
        Task<PagedResult2<SaleOrderLineBasic>> GetPagedResultAsync(SaleOrderLinesPaged val);
        Task Unlink(IEnumerable<Guid> ids);
        Task _UpdateInvoiceQty(IEnumerable<Guid> ids);
        void UpdateProps(IEnumerable<SaleOrderLine> self);
        AccountMoveLine _PrepareInvoiceLine(SaleOrderLine line);

        Task CancelSaleOrderLine(IEnumerable<Guid> Ids);

        Task<IEnumerable<LaboOrderBasic>> GetLaboOrderBasics(Guid id);

        Task<IEnumerable<ToothBasic>> GetTeeth(Guid id);

        void _ComputeLinePaymentRels(IEnumerable<SaleOrderLine> lines);
        Task _RemovePartnerCommissions(IEnumerable<Guid> ids);

        Task RecomputeCommissions(IEnumerable<SaleOrderLine> self);

        Task<IEnumerable<SaleOrderLine>> _ComputePaidResidual(IEnumerable<Guid> ids);

        Task<IEnumerable<SaleOrderLineDisplay>> GetDisplayBySaleOrder(Guid Id);
        Task UpdateDkByOrderLine(Guid key, SaleOrderLineDotKhamSave val);
        void ComputeResidual(IEnumerable<SaleOrderLine> self);

        Task ComputeProductRequestedQuantity(IEnumerable<Guid> ids);

        void _ComputeAmountDiscountTotal(IEnumerable<SaleOrderLine> self);

        void RecomputePromotionLine(IEnumerable<SaleOrderLine> self);

        SaleOrderPromotion _GetRewardLineValues(SaleOrderLine self, SaleCouponProgram program);

        SaleOrderPromotion _GetRewardValuesDiscount(SaleOrderLine self, SaleCouponProgram program);

        Task ApplyDiscountOnOrderLine(ApplyDiscountViewModel val);

        Task<SaleCouponProgramResponse> ApplyPromotionUsageCodeOnOrderLine(ApplyPromotionUsageCode val);

        Task ApplyPromotionOnOrderLine(ApplyPromotionRequest val);

        decimal _GetRewardValuesDiscountPercentagePerLine(SaleCouponProgram program, SaleOrderLine line);

        Task<PagedResult2<SmsCareAfterOrder>> GetPagedSmsCareAfterOrderAsync(SmsCareAfterOrderPaged val);
        Task<IEnumerable<ProductSimple>> GetProductSmsCareAfterOrder(DateTime? dateFrom, DateTime? dateTo, Guid? companyId);
    }
}
