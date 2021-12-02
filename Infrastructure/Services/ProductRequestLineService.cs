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

        public async Task<ProductRequestLineDisplay> GetlineAble(ProductRequestGetLinePar val)
        {
            var bomObj = GetService<IProductBomService>();
            var orderLineObj = GetService<ISaleOrderLineService>();
            var saleProductionLineObj = GetService<ISaleProductionLineService>();

            //validate số lượng
            var productionLine = await saleProductionLineObj.SearchQuery(x => x.Id == val.SaleProductionLineId).Include(x => x.Product).ThenInclude(x => x.UOM).FirstOrDefaultAsync();
            if (productionLine == null)
                throw new Exception("Không tồn tại vật tư");

            //var requestedObj = GetService<ISaleOrderLineProductRequestedService>();
            //var requested = await requestedObj.SearchQuery(x => x.ProductId == val.ProductBomId && x.SaleOrderLineId == val.SaleOrderLineId).FirstOrDefaultAsync();// da su dung
            if (productionLine != null && productionLine.QuantityRequested >= productionLine.Quantity)
                throw new Exception("Không còn định mức vật tư cho dịch vụ");

            //return line.
            //var orderLine = await orderLineObj.SearchQuery(x => x.Id == val.SaleOrderLineId).FirstOrDefaultAsync();
            //if (orderLine == null)
            //    throw new Exception("Không tìm thấy dịch vụ");

            return new ProductRequestLineDisplay()
            {
                ProductId = productionLine.ProductId,
                Product = _mapper.Map<ProductSimple>(productionLine.Product),
                ProductQty = 1,
                ProductUOMId = productionLine.Product.UOMId,
                ProducUOM = _mapper.Map<UoMSimple>(productionLine.Product.UOM),
                SaleProductionLineId = productionLine.Id
            };
        }
      
    }
}
