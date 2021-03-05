using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductRequestLineService : BaseService<ProductRequestLine> , IProductRequestLineService
    {
        private readonly IMapper _mapper;
        public ProductRequestLineService(IAsyncRepository<ProductRequestLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ProductRequestLineDisplay> GetlineAble(GetLinePar val)
        {
            var bomObj = GetService<IProductBomService>();
            var orderLineObj = GetService<ISaleOrderLineService>();

            //validate số lượng
            var bom = await bomObj.SearchQuery(x => x.Id == val.ProductBomId).Include(x => x.ProductUOM).Include(x => x.MaterialProduct).FirstOrDefaultAsync();
            if (bom == null)
                throw new Exception("Không tồn tại vật tư");

            var sum = await SearchQuery(x => x.ProductId == val.ProductBomId && x.SaleOrderLineId == val.SaleOrderLineId && x.Request.State != "draft").SumAsync(x => x.ProductQty);// da su dung
            if (sum >= bom.Quantity)
                throw new Exception("Không còn định mức vật tư cho dịch vụ");

            //return line.
            var orderLine = await orderLineObj.SearchQuery(x => x.Id == val.SaleOrderLineId).FirstOrDefaultAsync();
            if (orderLine == null)
                throw new Exception("Không tìm thấy dịch vụ");

            return new ProductRequestLineDisplay()
            {
                ProductId = bom.MaterialProductId,
                Product = _mapper.Map<ProductSimple>(bom.MaterialProduct),
                ProductQty = 0,
                ProductQtyMax = bom.Quantity - sum,
                ProductUOMId = bom.ProductUOMId,
                ProducUOM = _mapper.Map<UoMSimple>(bom.ProductUOM),
                SaleOrderLineId = val.SaleOrderLineId,
                SaleOrderLine = _mapper.Map<SaleOrderLineSimple>(orderLine),
            };
        }

        public async Task SaveUpdateRequestedQuantity(List<Guid> ids, List<ProductRequestLine> selfs = null, bool isCU = true)
        {
            selfs = selfs == null ? await SearchQuery(x => ids.Contains(x.Id)).ToListAsync() : selfs;

            //luu lai quantity da yeu cau
            //check đã tồn tại thì update quantity, else create, neu ko phai isCU thi giam quantity, neu quantity =0 thi xoa
            var requestedObj = GetService<ISaleOrderLineProductRequestedService>();
            var toCreates = new List<SaleOrderLineProductRequested>();
            var toUpdates = new List<SaleOrderLineProductRequested>();
            var toRemoves = new List<SaleOrderLineProductRequested>();
            foreach (var line in selfs)
            {
                var exist = await requestedObj.SearchQuery(x => x.SaleOrderLineId == line.SaleOrderLineId && x.ProductId == line.ProductId).FirstOrDefaultAsync();
                if (exist == null)
                {
                    toCreates.Add(new SaleOrderLineProductRequested()
                    { ProductId = line.ProductId.Value, SaleOrderLineId = line.SaleOrderLineId.Value, RequestedQuantity = line.ProductQty });
                }
                else
                {
                    exist.RequestedQuantity = isCU == true ? (exist.RequestedQuantity + line.ProductQty) : (exist.RequestedQuantity - line.ProductQty);
                    if (exist.RequestedQuantity <= 0)
                        toRemoves.Add(exist);
                    else
                        toUpdates.Add(exist);
                }
            }
        }
    }
}
