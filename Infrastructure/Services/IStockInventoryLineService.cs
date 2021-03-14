using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IStockInventoryLineService: IBaseService<StockInventoryLine>
    {
        Task<IEnumerable<StockMove>> ResolveInventoryLine(StockInventoryLine line);
        Task _GenerateMoves(IEnumerable<StockInventoryLine> self);
        Task<StockInventoryLineDisplay> OnChangeCreateLine(StockInventoryLineOnChangeCreateLine val);
    }
}
