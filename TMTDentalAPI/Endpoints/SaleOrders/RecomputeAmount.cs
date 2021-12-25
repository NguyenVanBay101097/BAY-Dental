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

namespace TMTDentalAPI.Endpoints.SaleOrders
{
    public class RecomputeAmount : BaseAsyncEndpoint
      .WithoutRequest
      .WithoutResponse
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly ISaleOrderLineService _saleLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;
     
        public RecomputeAmount(ISaleOrderService saleOrderService,
            ISaleOrderLineService saleLineService,
            IUnitOfWorkAsync unitOfWork)
        {
            _saleOrderService = saleOrderService;
            _saleLineService = saleLineService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("api/SaleOrderRecomputeAmountAll")]
        public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync();
            //tim nhung sale order can tinh toan lai
            var orders = await _saleOrderService.SearchQuery(x => x.State != "draft" && ((x.AmountTotal - x.TotalPaid) != x.Residual))
                .Include(x => x.OrderLines)
                .ToListAsync();

            foreach(var order in orders)
            {
                _saleLineService._GetInvoiceAmount(order.OrderLines);
                await _saleLineService.UpdateAsync(order.OrderLines);
            }

            _saleOrderService._AmountAll(orders);
            await _saleOrderService.UpdateAsync(orders);

            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
