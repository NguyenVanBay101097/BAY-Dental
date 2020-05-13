using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPurchaseOrderLineService: IBaseService<PurchaseOrderLine>
    {
        Task<PurchaseOrderLineOnChangeProductResult> OnChangeProduct(PurchaseOrderLineOnChangeProduct val);
        Task<PurchaseOrderLineOnChangeProductResult> OnChangeUoMProduct(PurchaseOrderLineOnChangeProduct val);
        void _ComputeAmount(IEnumerable<PurchaseOrderLine> self);
        void _ComputeQtyInvoiced(IEnumerable<PurchaseOrderLine> self);
        AccountMoveLine _PrepareAccountMoveLine(PurchaseOrderLine self, AccountMove move);
    }
}
