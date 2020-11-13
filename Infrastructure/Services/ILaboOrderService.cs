using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILaboOrderService: IBaseService<LaboOrder>
    {
        Task<PagedResult2<LaboOrderBasic>> GetPagedResultAsync(LaboOrderPaged val);
        Task<PagedResult2<LaboOrderBasic>> GetFromSaleOrder_OrderLine(LaboOrderPaged val);

        Task<LaboOrderDisplay> GetLaboDisplay(Guid id);
        Task<LaboOrder> CreateLabo(LaboOrderDisplay val);
        Task UpdateLabo(Guid id, LaboOrderDisplay val);
        Task Unlink(IEnumerable<Guid> ids);
        Task<LaboOrderDisplay> DefaultGet(LaboOrderDefaultGet val);
        Task ButtonConfirm(IEnumerable<Guid> ids);
        Task ButtonCancel(IEnumerable<Guid> ids);
        Task<LaboOrderPrintVM> GetPrint(Guid id);
        Task<IEnumerable<LaboOrderBasic>> GetAllForDotKham(Guid dotKhamId);

        Task<PagedResult2<LaboOrderStatisticsBasic>> GetStatisticsPaged(LaboOrderStatisticsPaged val);

        Task<LaboOrderReportOutput> GetLaboOrderReport(LaboOrderReportInput val);
    }
}
