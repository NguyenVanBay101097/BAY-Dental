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
    }
}
