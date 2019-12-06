using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderLineService: IBaseService<SaleOrderLine>
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
        void _ComputeInvoiceStatus(IEnumerable<SaleOrderLine> lines);
        void _GetInvoiceQty(IEnumerable<SaleOrderLine> lines);
        AccountInvoiceLine _PrepareInvoiceLine(SaleOrderLine line, decimal qty, AccountAccount account);
        Task<PagedResult2<SaleOrderLine>> GetPagedResultAsync(SaleOrderLinesPaged val);
        Task Unlink(IEnumerable<Guid> ids);
    }
}
