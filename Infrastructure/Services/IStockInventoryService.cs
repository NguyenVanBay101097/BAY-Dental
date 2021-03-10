using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IStockInventoryService : IBaseService<StockInventory>
    {
        Task<PagedResult2<StockInventoryBasic>> GetPagedResultAsync(StockInventoryPaged val);
        Task<StockInventoryDisplay> GetDisplay(Guid id);
        Task<IEnumerable<ProductStockInventory>> GetListProductInventory(Guid id);
        Task<StockInventoryDisplay> DefaultGet();
        Task<StockInventory> CreateStockInventory(StockInventorySave val);
        Task UpdateStockInventory(Guid id, StockInventorySave val);



        Task<StockInventoryLineDisplay> InventoryLineByProductId(StockInventoryLineByProductId val);

        Task PrepareInventory(IEnumerable<Guid> ids);
        Task ActionDone(IEnumerable<Guid> ids);

        Task ActionCancel(IEnumerable<Guid> ids);
    }
}
