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
    public class UpdateService : BaseAsyncEndpoint
    .WithRequest<UpdateServiceRequest>
    .WithoutResponse
    {
        private readonly IProductService _productService;
        private readonly IIRPropertyService _propertyService;
        private readonly ICurrentUser _currentUser;
        private readonly IProductPriceHistoryService _priceHistoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRSequenceService _sequenceService;
        public UpdateService(IProductService productService,
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

        [HttpPut("api/ServiceProducts")]
        public override async Task<ActionResult> HandleAsync(UpdateServiceRequest request, CancellationToken cancellationToken = default)
        {
            var product = await _productService.SearchQuery(x => x.Id == request.Id)
                .Include(x => x.Boms)
                .Include(x => x.Steps)
                .FirstOrDefaultAsync();

            if (product.IsLabo)
            {
                product.Firm = request.Firm;
                product.LaboPrice = request.LaboPrice ?? 0;
            }

            var existSteps = product.Steps.ToList();
            var stepToRemoves = new List<ProductStep>();
            foreach (var existStep in existSteps)
            {
               if (!request.Steps.Any(x => x.Id == existStep.Id))
                    stepToRemoves.Add(existStep);
            }

            foreach (var line in stepToRemoves)
                product.Steps.Remove(line);

            int sequence = 0;
            foreach (var line in request.Steps)
            {
                if (!line.Id.HasValue)
                {
                    product.Steps.Add(new ProductStep
                    {
                        Order = sequence++,
                        Name = line.Name
                    });
                }
                else
                {
                    var sl = product.Steps.SingleOrDefault(c => c.Id == line.Id);
                    if (sl != null)
                    {
                        sl.Order = sequence++;
                        sl.Name = line.Name;
                    }
                }
            }

            var existBoms = product.Boms.ToList();
            var bomToRemoves = new List<ProductBom>();
            foreach (var existBom in existBoms)
            {
                if (!request.Boms.Any(x => x.Id == existBom.Id))
                    bomToRemoves.Add(existBom);
            }


            foreach (var bom in bomToRemoves)
                product.Boms.Remove(bom);

            int sequence2 = 0;
            foreach (var line in request.Boms)
            {
                if (!line.Id.HasValue)
                {
                    var item = new ProductBom 
                    { 
                        MaterialProductId = line.MaterialProductId,
                        ProductUOMId = line.ProductUomId,
                        Quantity = line.Quantity,
                        Sequence = sequence2++
                    };

                    product.Boms.Add(item);
                }
                else
                {
                    var item = product.Boms.SingleOrDefault(c => c.Id == line.Id);
                    if (item != null)
                    {
                        item.MaterialProductId = line.MaterialProductId;
                        item.ProductUOMId = line.ProductUomId;
                        item.Quantity = line.Quantity;
                        item.Sequence = sequence2++;
                    }
                }
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

            await _productService.UpdateAsync(product);

            var oldStdPrice = (decimal)Convert.ToDouble(_propertyService.get("standard_price", "product.product", res_id: $"product.product,{product.Id}"));
            var newStdPrice = request.StandardPrice ?? 0;
            if (newStdPrice != oldStdPrice)
            {
                var list = new List<Product>() { product };
                //Store the standard price change in order to be able to retrieve the cost of a product for a given date'''
                _propertyService.set_multi("standard_price", "product.product", list.ToDictionary(x => string.Format("product.product,{0}", x.Id), x => (object)newStdPrice), force_company: _currentUser.CompanyId);

                await _priceHistoryService.CreateAsync(new ProductPriceHistory
                {
                    ProductId = product.Id,
                    Cost = (double)newStdPrice,
                    CompanyId = _currentUser.CompanyId ?? Guid.Empty
                });
            }

            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
