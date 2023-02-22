using ApplicationCore.Entities;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.SaleOrders
{
    [Authorize]
    public class Create : BaseAsyncEndpoint
       .WithRequest<CreateSaleOrderRequest>
       .WithResponse<CreateSaleOrderResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderService _saleOrderService;
        private readonly ISaleOrderLineService _saleOrderLineService;


        [HttpPost("api/saleorders/Create")]
        [SwaggerOperation(
        Summary = "Creates a new Catalog Item",
        Description = "Creates a new Catalog Item",
        OperationId = "catalog-items.create",
        Tags = new[] { "CatalogItemEndpoints" })
        ]

        public override async Task<ActionResult<CreateSaleOrderResponse>> HandleAsync(CreateSaleOrderRequest request, CancellationToken cancellationToken = default)
        {
            var order = _mapper.Map<SaleOrder>(request);
            await _saleOrderService.CreateAsync(order);

            var lines = new List<SaleOrderLine>();
            var sequence = 0;
            foreach (var item in request.OrderLines)
            {
                var saleLine = _mapper.Map<SaleOrderLine>(item);
                saleLine.Order = order;
                saleLine.Sequence = sequence++;
                saleLine.AmountResidual = saleLine.PriceSubTotal - saleLine.AmountPaid;

                if (item.ToothType == "manual")
                {
                    foreach (var toothId in item.ToothIds)
                    {
                        saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                        {
                            ToothId = toothId
                        });
                    }
                }

                lines.Add(saleLine);
            }

            await _saleOrderLineService.CreateAsync(lines);

            _saleOrderService._AmountAll(order);
            await _saleOrderService.UpdateAsync(order);

            return _mapper.Map<CreateSaleOrderResponse>(order);
        }
    }
}
