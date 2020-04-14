using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILaboOrderLineService: IBaseService<LaboOrderLine>
    {
        Task<PagedResult2<LaboOrderLineBasic>> GetPagedResultAsync(LaboOrderLinePaged val);
        Task<LaboOrderLine> GetLaboLineForDisplay(Guid id);
        Task<LaboOrderLineDisplay> DefaultGet(LaboOrderLineDefaultGet val);
        Task<LaboOrderLineOnChangeProductResult> OnChangeProduct(LaboOrderLineOnChangeProduct val);
        Task<IEnumerable<LaboOrderLineBasic>> GetAllForDotKham(Guid dotKhamId);
        void _ComputeAmount(IEnumerable<LaboOrderLine> self);
        void _ComputeQtyInvoiced(IEnumerable<LaboOrderLine> self);
        AccountMoveLine _PrepareAccountMoveLine(LaboOrderLine self, AccountMove move);
    }
}
