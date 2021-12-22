using ApplicationCore.Entities;
using ApplicationCore.Users;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.ProductEndpoints
{
    [Authorize]
    public class GetService : BaseAsyncEndpoint
    .WithRequest<GetServiceRequest>
    .WithResponse<GetServiceResponse>
    {
        private readonly IProductService _productService;
        private readonly IIRPropertyService _propertyService;
        private readonly ICurrentUser _currentUser;
        private readonly IProductPriceHistoryService _priceHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRSequenceService _sequenceService;
        private readonly IProductBomService _productBomService;
        public GetService(IProductService productService,
            IIRPropertyService propertyService,
            ICurrentUser currentUser,
            IProductPriceHistoryService priceHistoryService,
            IUnitOfWorkAsync unitOfWork,
            IIRSequenceService sequenceService,
            IProductBomService productBomService)
        {
            _productService = productService;
            _propertyService = propertyService;
            _currentUser = currentUser;
            _priceHistoryService = priceHistoryService;
            _unitOfWork = unitOfWork;
            _sequenceService = sequenceService;
            _productBomService = productBomService;
        }

        [HttpGet("api/ServiceProducts/{Id}")]
        public override async Task<ActionResult<GetServiceResponse>> HandleAsync([FromRoute]GetServiceRequest request, CancellationToken cancellationToken = default)
        {
            var product = await _productService.SearchQuery(x => x.Id == request.Id)
                .Include(x => x.Steps)
                .Include(x => x.Categ)
                .Include(x => x.UOM)
                .FirstOrDefaultAsync();

            var boms = await _productBomService.SearchQuery(x => x.ProductId == request.Id)
                .OrderBy(x => x.Sequence)
                .Include(x => x.MaterialProduct)
                .Include(x => x.ProductUOM)
                .ToListAsync();

            var result = new GetServiceResponse()
            {
                Id = product.Id,
                Name = product.Name,
                Categ = new ProductCategorySimple
                {
                    Id = product.Categ.Id,
                    Name = product.Categ.Name
                },
                DefaultCode = product.DefaultCode,
                Firm = product.Firm,
                IsLabo = product.IsLabo,
                LaboPrice = product.LaboPrice,
                ListPrice = product.ListPrice,
                UOM = new UoMSimple
                {
                    Id = product.UOM.Id,
                    Name = product.UOM.Name
                },
                Steps = product.Steps.Select(x => new ProductStepSimple { 
                    Id = x.Id,
                    Name = x.Name
                }),
                Boms = boms.Select(x => new ProductBomBasic { 
                    Id = x.Id,
                    MaterialProduct = new ProductSimple
                    {
                        Id = x.MaterialProduct.Id,
                        Name = x.MaterialProduct.Name,
                    },
                    ProductUOM = new UoMSimple
                    {
                        Id = x.ProductUOM.Id,
                        Name = x.ProductUOM.Name,
                    },
                    Quantity = x.Quantity
                })
            };

            var stdPrice = Convert.ToDouble(_propertyService.get("standard_price", "product.product", res_id: $"product.product,{product.Id}"));
            result.StandardPrice = (decimal)stdPrice;

            return result;
        }
    }
}
