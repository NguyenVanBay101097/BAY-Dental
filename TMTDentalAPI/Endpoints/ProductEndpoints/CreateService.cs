using ApplicationCore.Entities;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.ProductEndpoints
{
    public class CreateService : BaseAsyncEndpoint
    .WithRequest<CreateServiceRequest>
    .WithResponse<CreateServiceResponse>
    {
        private readonly IProductService _productService;
        private readonly IIRPropertyService _propertyService;
        public CreateService(IProductService productService,
            IIRPropertyService propertyService)
        {
            _productService = productService;
            _propertyService = propertyService;
        }
        [HttpPost("api/ServiceProducts")]
        public override async Task<ActionResult<CreateServiceResponse>> HandleAsync(CreateServiceRequest request, CancellationToken cancellationToken = default)
        {
            var product = new Product()
            { 
                Type = "service",
                Type2 = "service",
                Name = request.Name,
                DefaultCode = request.DefaultCode,
                SaleOK = true,
                IsLabo = request.IsLabo,
                CategId = request.CategId,
                ListPrice = request.ListPrice ?? 0,
            };

            if (product.IsLabo)
            {
                product.Firm = request.Firm;
                product.LaboPrice = request.LaboPrice ?? 0;
            }

            var stdPrice = request.StandardPrice ?? 0;
            if (stdPrice > 0)
            {
                var list = new List<Product>() { product };
                //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
                _propertyService.set_multi("standard_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)stdPrice), force_company: force_company);

                //var priceHistoryObj = GetService<IProductPriceHistoryService>();
                //priceHistoryObj.Create(new ProductPriceHistory
                //{
                //    ProductId = self.Id,
                //    Cost = value,
                //    CompanyId = CompanyId
                //});
            }

        

            //_SaveProductSteps(product, val.StepList);

            //_SaveProductBoms(product, val.Boms);

            //_SaveUoMRels(product, val);

            //UpdateProductCriteriaRel(product, val);

            //_ComputeUoMRels(new List<Product>() { product });

            //_SetNameNoSign(new List<Product>() { product });

            //await _GenerateCodeIfEmpty(new List<Product>() { product });

            //product = await CreateAsync(product);

            //SetStandardPrice(product, val.StandardPrice, force_company: product.CompanyId);

            //await _CheckProductExistCode(new List<Product>() { product });

            //return product;

            throw new NotImplementedException();
        }
    }
}
