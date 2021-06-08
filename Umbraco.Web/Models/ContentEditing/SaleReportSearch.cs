using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportSearch
    {
        //báo cáo group by theo khách hàng, sản phẩm, nhân viên, thời gian: ngày, tuần, tháng, năm
        //search, thời gian, trạng thái
        //sum số lượng, tổng tiền
        //detail show chi tiết

        //PartnerId, ProductId, UserId, Date:day, Date:week, Date:month, Date:year
        public string GroupBy { get; set; }

        public string Search { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo{ get; set; }

        public string State { get; set; }

        public bool? IsQuotation { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class GetSummarySaleReportRequest
    {
        public Guid? PartnerId { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class GetSummarySaleReportResponse
    {
        public GetSummarySaleReportResponse()
        {
            PaidTotal = 0;
            ResidualTotal = 0;
        }

        public decimal PriceTotal { get; set; }

        public decimal? PaidTotal { get; set; }

        public decimal? ResidualTotal { get; set; }

        public decimal QtyTotal { get; set; }
    }
}
