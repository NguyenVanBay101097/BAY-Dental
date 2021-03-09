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

            var requestedObj = GetService<ISaleOrderLineProductRequestedService>();
            var requested = await requestedObj.SearchQuery(x => x.ProductId == val.ProductBomId && x.SaleOrderLineId == val.SaleOrderLineId).FirstOrDefaultAsync();// da su dung
            if (requested != null && requested.RequestedQuantity >= bom.Quantity)
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
                ProductUOMId = bom.ProductUOMId,
                ProducUOM = _mapper.Map<UoMSimple>(bom.ProductUOM),
                SaleOrderLineId = val.SaleOrderLineId,
                SaleOrderLine = _mapper.Map<SaleOrderLineSimple>(orderLine),
            };
        }
      
    }
}
