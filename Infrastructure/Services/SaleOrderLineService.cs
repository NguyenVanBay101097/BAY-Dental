using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleOrderLineService : BaseService<SaleOrderLine>, ISaleOrderLineService
    {
        private readonly IMapper _mapper;
        public SaleOrderLineService(IAsyncRepository<SaleOrderLine> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public void ComputeAmount(ICollection<SaleOrderLine> self)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {
                var price = line.PriceUnit * (1 - line.Discount / 100);
                line.PriceTax = 0;
                line.PriceSubTotal = price * line.ProductUOMQty;
                line.PriceTotal = line.PriceSubTotal + line.PriceTax;
            }
        }

        public async Task<SaleOrderLineDisplay> OnChangeProduct(SaleOrderLineDisplay val)
        {
            if (val.Product != null)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.GetByIdAsync(val.Product.Id);
                val.Name = product.Name;
                val.PriceUnit = product.ListPrice;
            }

            return val;
        }

        public void UpdateOrderInfo(ICollection<SaleOrderLine> self, SaleOrder order)
        {
            if (self == null)
                return;

            foreach (var line in self)
            {
                line.SalesmanId = order.UserId;
                line.OrderPartnerId = order.PartnerId;
                line.CompanyId = order.CompanyId;
            }
        }
    }
}
