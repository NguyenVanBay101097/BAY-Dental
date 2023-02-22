using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.CommissionSettlements
{
    public class GetReportCommissionSettlementResponse
    {

        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        /// <summary>
        /// số tiền thanh toán
        /// </summary>
        public decimal? BaseAmount { get; set; }

        /// <summary>
        /// phần trăm hoa hồng
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// tiền hoa hồng bác sĩ
        /// </summary>
        public decimal? Amount { get; set; }

        //lưu lại điều kiện lọc
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
