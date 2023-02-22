using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IStockQuantService: IBaseService<StockQuant>
    {
        Task<IList<QuantOrder>> QuantsGetReservation(decimal qty, StockMove move);
        Task QuantsMove(IList<QuantOrder> quants, StockMove move, StockLocation locationDest,
            StockLocation locationFrom = null);
    }
}
