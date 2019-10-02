using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IStockPickingService: IBaseService<StockPicking>
    {
        Task ActionDone(IEnumerable<Guid> ids);
        Task<StockPickingDisplay> DefaultGet(StockPickingDefaultGet val);
        Task<PagedResult2<StockPickingBasic>> GetPagedResultAsync(StockPickingPaged val);
        Task<StockPickingOnChangePickingTypeResult> OnChangePickingType(StockPickingOnChangePickingType val);
        Task<StockPicking> GetPickingForDisplay(Guid id);
    }
}
