using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<Guid> ServiceIds { get; set; } = new List<Guid>();

    }
    public class ServiceReportRes
    {
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? ProductId { get; set; }
    }

    public class ServiceOverviewResponse
    {
        public DateTime? Date { get; set; }

        public string Name { get; set; }

        public Guid OrderId { get; set; }
        public string OrderName { get; set; }

        public Guid? PartnerId { get; set; }
        public string PartnerName { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string EmployeeName { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal AmountResidual { get; set; }

        public string State { get; set; }

        public string StateDisplay
        {
            get
            {
                switch (State)
                {
                    case "sale":
                        return "Đang điều trị";
                    case "cancel":
                        return "Ngừng điều trị";
                    case "done":
                        return "Hoàn thành";
                    default:
                        return "Nháp";
                }
            }
        }


    }

    public class PrintServiceOverviewResponse
    {
        public IEnumerable<SaleOrderLineBasic> data { get; set; } = new List<SaleOrderLineBasic>();
        public object Aggregates { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
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
        public IEnumerable<Guid> ServiceIds { get; set; } = new List<Guid>();

    }

    public class ServiceReportDetailRes
    {
        public DateTime? Date { get; set; }
        public string OrderPartnerId { get; set; }
        public string OrderPartnerName { get; set; }
        public string OrderPartnerDisplayName { get; set; }
        public string Name { get; set; }
        public string EmployeeName { get; set; }
        public string ToothType { get; set; }
        public IEnumerable<ToothSimple> Teeth { get; set; } = new List<ToothSimple>();
        public string TeethDisplay
        {
            get
            {
                switch (ToothType)
                {
                    case "whole_jaw":
                        return "Nguyên hàm";
                    case "upper_jaw":
                        return "Hàm trên";
                    case "lower_jaw":
                        return "Hàm dưới";
                    default:
                        return string.Join(", ", Teeth.Select(x => x.Name));
                }
            }
        }
        public decimal ProductUOMQty { get; set; }
        public decimal PriceSubTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountResidual { get; set; }
        public string State { get; set; }
        public string StateDisplay
        {
            get
            {
                switch (State)
                {
                    case "sale":
                        return "Đang điều trị";
                    case "cancel":
                        return "Ngừng điều trị";
                    case "done":
                        return "Hoàn thành";
                    default:
                        return "Nháp";
                }
            }
        }
        public string OrderId { get; set; }
        public string OrderName { get; set; }
        public Guid ProductId { get; set; }
        public UoMBasic ProductUOM { get; set; }

    }

    public class ServiceReportResPrint : ServiceReportRes
    {
        public IEnumerable<ServiceReportDetailRes> Lines { get; set; } = new List<ServiceReportDetailRes>();
    }

    public class ServiceReportPrint
    {
        public IEnumerable<ServiceReportResPrint> data { get; set; } = new List<ServiceReportResPrint>();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
        public string Title { get; set; }
        //time, service
        public string type { get; set; }
    }

    public class ServiceReportResExcel : ServiceReportRes
    {
        public IEnumerable<ServiceReportDetailRes> Lines { get; set; } = new List<ServiceReportDetailRes>();
    }
}
