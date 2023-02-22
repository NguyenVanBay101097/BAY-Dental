using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.CommissionSettlements
{
    public class GetReportCommissionSettlementRequest
    {
        public GetReportCommissionSettlementRequest()
        {
            Limit = 20;
        }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CompanyId { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }
    }
}
