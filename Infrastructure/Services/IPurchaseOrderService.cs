using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPurchaseOrderService: IBaseService<PurchaseOrder>
    {
        Task<PagedResult2<PurchaseOrderBasic>> GetPagedResultAsync(PurchaseOrderPaged val);
        Task<PurchaseOrderDisplay> GetPurchaseDisplay(Guid id);
        Task<PurchaseOrder> CreatePurchaseOrder(PurchaseOrderSave val);
        Task UpdatePurchaseOrder(Guid id, PurchaseOrderSave val);
        Task Unlink(IEnumerable<Guid> ids);
        PurchaseOrderDisplay DefaultGet(PurchaseOrderDefaultGet val);
        Task ButtonConfirm(IEnumerable<Guid> ids);
        Task ButtonCancel(IEnumerable<Guid> ids);
        Task PreparePurchase(IEnumerable<Guid> ids);
    }
}
