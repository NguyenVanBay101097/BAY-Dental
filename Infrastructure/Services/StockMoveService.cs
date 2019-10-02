using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class StockMoveService : BaseService<StockMove>, IStockMoveService
    {

        public StockMoveService(IAsyncRepository<StockMove> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task ActionDone(IEnumerable<StockMove> self)
        {
            foreach (var move in self)
            {
                if (move.ProductUOMQty <= 0)
                    continue;
                var quantObj = GetService<IStockQuantService>();
                var quants = await quantObj.QuantsGetReservation(move.ProductUOMQty, move);
                await quantObj.QuantsMove(quants, move, move.LocationDest);
                move.State = "done";
            }

            await UpdateAsync(self);
        }

        public async Task<StockMoveOnChangeProductResult> OnChangeProduct(StockMoveOnChangeProduct val)
        {
            var res = new StockMoveOnChangeProductResult();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.GetByIdAsync(val.ProductId);
                res.Name = product.Name;
            }
            return res;
        }
    }
}
