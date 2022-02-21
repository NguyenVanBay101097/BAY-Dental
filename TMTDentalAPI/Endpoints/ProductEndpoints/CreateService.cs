using ApplicationCore.Entities;
using ApplicationCore.Users;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ICurrentUser _currentUser;
        private readonly IProductPriceHistoryService _priceHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRSequenceService _sequenceService;
        public CreateService(IProductService productService,
            IIRPropertyService propertyService,
            ICurrentUser currentUser,
            IProductPriceHistoryService priceHistoryService,
            IUnitOfWorkAsync unitOfWork,
            IIRSequenceService sequenceService)
        {
            _productService = productService;
            _propertyService = propertyService;
            _currentUser = currentUser;
            _priceHistoryService = priceHistoryService;
            _unitOfWork = unitOfWork;
            _sequenceService = sequenceService;
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
                CompanyId = _currentUser.CompanyId,
                UOMId = request.UOMId,
                UOMPOId = request.UOMId
            };

            if (product.IsLabo)
            {
                product.Firm = request.Firm;
                product.LaboPrice = request.LaboPrice ?? 0;
            }

            int sequence = 0;
            foreach (var line in request.Steps)
            {
                product.Steps.Add(new ProductStep
                {
                    Order = sequence++,
                    Name = line.Name
                });
            }

            int sequence2 = 0;
            foreach (var line in request.Boms)
            {
                product.Boms.Add(new ProductBom
                { 
                    MaterialProductId = line.MaterialProductId,
                    Quantity = line.Quantity,
                    ProductUOMId = line.ProductUomId,
                    Sequence = sequence2++
                });
            }

            await _unitOfWork.BeginTransactionAsync();

            if (product.DefaultCode.IsNullOrWhiteSpace())
            {
                var matchedCodes = await _productService.SearchQuery(x => x.Type2 == "service" && !string.IsNullOrEmpty(x.DefaultCode)).Select(x => x.DefaultCode).ToListAsync();
                foreach (var num in Enumerable.Range(1, 100))
                {
                    var productCode = await _sequenceService.NextByCode("product_seq");
                    if (!matchedCodes.Contains(productCode))
                    {
                        product.DefaultCode = productCode;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(product.DefaultCode))
                    throw new Exception("Không thể phát sinh mã");
            }

            await _productService.CreateAsync(product);

            var stdPrice = request.StandardPrice ?? 0;
            if (stdPrice > 0)
            {
                var list = new List<Product>() { product };
                //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
                _propertyService.set_multi("standard_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)stdPrice), force_company: _currentUser.CompanyId);

                await _priceHistoryService.CreateAsync(new ProductPriceHistory
                {
                    ProductId = product.Id,
                    Cost = (double)stdPrice,
                    CompanyId = _currentUser.CompanyId ?? Guid.Empty
                });
            }
          
            _unitOfWork.Commit();

            return new CreateServiceResponse { Id = product.Id };
        }
    }
}
