using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Features.PartnerAdvanceReport
{
    public class GetSummaryHandler : IRequestHandler<PartnerAdvanceReportGetSummary, PartnerAdvanceReportSummaryResponse>
    {
        private readonly IAccountMoveLineService _amlService;

        public GetSummaryHandler(IAccountMoveLineService amlService)
        {
            _amlService = amlService;
        }

        public async Task<PartnerAdvanceReportSummaryResponse> Handle(PartnerAdvanceReportGetSummary request, CancellationToken cancellationToken)
        {
            var query = _amlService._QueryGet(state: "posted", companyId: request.CompanyId);
            if (request.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == request.PartnerId);

            var tuQuery = query.Where(x => x.Account.Code == "KHTU");
            var htuQuery = query.Where(x => x.Account.Code == "HTU");

            var res = new PartnerAdvanceReportSummaryResponse
            {
                TotalDebit = await tuQuery.SumAsync(x => x.Debit),
                TotalCredit = await tuQuery.SumAsync(x => x.Credit),
                TotalRefund = await htuQuery.SumAsync(x => x.Debit),
            };

            return res;
        }
    }
}
