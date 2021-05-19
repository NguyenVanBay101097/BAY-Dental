using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleOrderPaymentService : IBaseService<SaleOrderPayment>
    {
        Task<PagedResult2<SaleOrderPaymentBasic>> GetPagedResultAsync(SaleOrderPaymentPaged val);
        Task<PagedResult2<SaleOrderPaymentHistoryAdvance>> GetPagedResultHistoryAdvanceAsync(HistoryAdvancePaymentFilter val);

        Task<SaleOrderPayment> CreateSaleOrderPayment(SaleOrderPaymentSave val);

        Task<SaleOrderPaymentDisplay> GetDisplay(Guid id);
        Task ActionPayment(IEnumerable<Guid> ids);
        Task ActionCancel(IEnumerable<Guid> ids);

        Task Unlink(IEnumerable<Guid> ids);

        Task<SaleOrderPaymentPrintVM> GetPrint(Guid id);
    }
}
