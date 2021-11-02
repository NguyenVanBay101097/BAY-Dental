
using System;
using MediatR;
using System.Collections.Generic;

namespace TMTDentalAPI.Features.PartnerAdvanceReport
{
    public class PartnerAdvanceReportGetSummary : IRequest<PartnerAdvanceReportSummaryResponse>
    {
        public Guid? PartnerId { get; set; }

        public Guid? CompanyId { get; set; }

        public PartnerAdvanceReportGetSummary(Guid? partnerId, Guid? companyId)
        {
            PartnerId = partnerId;
            CompanyId = companyId;
        }
    }

    public class PartnerAdvanceReportSummaryResponse
    {
        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal TotalRefund { get; set; }

        public decimal TotalEnd
        {
            get
            {
                return TotalCredit - TotalDebit - TotalRefund;
            }
        }
    }
}

