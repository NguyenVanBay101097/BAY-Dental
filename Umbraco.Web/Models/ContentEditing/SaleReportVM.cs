using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceReportReq
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CompanyId { get; set; }
        public string State { get; set; }
        public string Search { get; set; }
        public bool? Active { get; set; }

    }
    public class ServiceReportRes
    {
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? ProductId { get; set; }
    }

    public class ServiceReportDetailReq
    {
        public ServiceReportDetailReq()
        {
            this.Limit = 20;
            this.Offset = 0;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CompanyId { get; set; }
        public string State { get; set; }
        public string Search { get; set; }
        public Guid? ProductId { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public bool? Active { get; set; }

    }

    public class ServiceReportDetailRes
    {
        public DateTime? OrderDateOrder { get; set; }
        public string OrderPartnerName { get; set; }
        public string Name { get; set; }
        public string EmployeeName { get; set; }
        public IEnumerable<ToothSimple> Teeth { get; set; } = new List<ToothSimple>();
        public decimal ProductUOMQty { get; set; }
        public decimal PriceSubTotal { get; set; }
        public string State { get; set; }
        public string OrderName { get; set; }
        public bool IsActive { get; set; }

    }
}
