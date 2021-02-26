using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IStockInventoryLineService: IBaseService<StockInventoryLine>
    {
        Task<IEnumerable<StockMove>> ResolveInventoryLine(StockInventoryLine line);
    }
}
