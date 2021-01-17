using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IStockMoveService: IBaseService<StockMove>
    {
        Task ActionDone(IEnumerable<StockMove> self);
        Task<StockMoveOnChangeProductResult> OnChangeProduct(StockMoveOnChangeProduct val);
        void _Compute(IEnumerable<StockMove> self);
    }
}
