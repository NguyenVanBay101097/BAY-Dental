using ApplicationCore.Utilities;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.CommissionSettlements
{
    [Authorize]
    public class GetReport : BaseAsyncEndpoint
       .WithRequest<GetReportCommissionSettlementRequest>
       .WithResponse<IEnumerable<GetReportCommissionSettlementResponse>>
    {
        private readonly ICommissionSettlementService _commissionSettlementService;
        public GetReport(ICommissionSettlementService commissionSettlementService)
        {
            _commissionSettlementService = commissionSettlementService;
        }


        [HttpPost("api/commissionsettlements/GetReport")]
        [SwaggerOperation(
        Summary = "Creates a new Catalog Item",
        Description = "Creates a new Catalog Item",
        OperationId = "catalog-items.create",
        Tags = new[] { "CatalogItemEndpoints" })
        ]

        public override async Task<ActionResult<IEnumerable<GetReportCommissionSettlementResponse>>> HandleAsync(GetReportCommissionSettlementRequest val, CancellationToken cancellationToken = default)
        {
            var query = _commissionSettlementService.SearchQuery();

            if (val.DateFrom.HasValue)
            {
                val.DateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= val.DateFrom);
            }

            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= val.DateTo);
            }


            if (val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);

            var result = await query
                .GroupBy(x => new
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.Name
                })
                .Select(x => new GetReportCommissionSettlementResponse
                {
                    EmployeeId = x.Key.EmployeeId,
                    EmployeeName = x.Key.EmployeeName,
                    BaseAmount = x.Sum(s => s.BaseAmount),
                    Percentage = x.Average(s => s.Percentage),
                    Amount = x.Sum(s => s.Amount),
                    CompanyId = val.CompanyId,
                    DateFrom = val.DateFrom,
                    DateTo = val.DateTo
                }).ToListAsync();

            return result;
        }
    }
}
