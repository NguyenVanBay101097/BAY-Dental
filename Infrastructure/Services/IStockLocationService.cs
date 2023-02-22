using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IStockLocationService: IBaseService<StockLocation>
    {
        Task<StockLocation> GetDefaultCustomerLocation();
        Task<StockLocation> GetDefaultSupplierLocation();
    }
}
