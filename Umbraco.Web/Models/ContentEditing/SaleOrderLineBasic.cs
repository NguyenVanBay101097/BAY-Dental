using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string State { get; set; }

        public Guid OrderPartnerId { get; set; }
        public PartnerSimple OrderPartner { get; set; }

        public SaleOrderSimple Order { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public string Diagnostic { get; set; }

        public DateTime DateCreated { get; set; }

        public EmployeeSimple Employee { get; set; }

        public EmployeeSimple Assistant { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();

        public bool IsListLabo { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        /// <summary>
        /// Số tiền đã thanh toán
        /// </summary>
        public decimal? AmountPaid { get; set; }

        public decimal? AmountInvoiced { get; set; }

        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manual :  thủ công
        /// </summary>
        public string ToothType { get; set; }
        public Guid? ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }

        public decimal? AmountResidual
        {
            get
            {
                return PriceTotal - (AmountInvoiced ?? 0);
            }
        }

        public bool IsActive { get; set; }
        public DateTime? Date { get; set; }

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
                        return "Mới";
                }
            }
        }
    }

    public class SaleOrderLineSmsSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }
    }

    public class ServiceSaleReportExcel// mẫu xuất excel báo cáo dịch vụ đang điều trị dựa vào saleorderlinebasic
    {
        [EpplusDisplay("Ngày tạo")]
        public DateTime DateCreated { get; set; }
        [EpplusDisplay("Khách hàng")]
        public string OrderPartnerName { get; set; }
        [EpplusDisplay("Dịch vụ")]
        public string Name { get; set; }
        [EpplusDisplay("Bác sĩ")]
        public string EmployeeName { get; set; }
        [EpplusIgnore]
        public UoMBasic ProductUOM { get; set; }
        [EpplusDisplay("Đơn vị tính")]
        public string ProductUOMName { 
            get
            {
                return ProductUOM != null ? ProductUOM.Name : "";
            }
            set { }
        }
        [EpplusIgnore]
        public string ToothType { get; set; }
        [EpplusIgnore]
        public string Teeth { get; set; }
        [EpplusDisplay("Răng")]
        public string TeethList
        {
            get
            {
                switch (this.ToothType)
                {
                    case "whole_jaw":
                        return "Nguyên hàm";
                    case "upper_jaw":
                        return "Hàm trên";
                    case "lower_jaw":
                        return "Hàm dưới";
                    default:
                        return this.Teeth;
                }
            }
            set { }
        }
        [EpplusDisplay("Số lượng")]
        public decimal ProductUOMQty { get; set; }
        [EpplusDisplay("Thành tiền")]
        public decimal PriceSubTotal { get; set; }
        [EpplusIgnore]
        public string State { get; set; }
        [EpplusIgnore]

        public bool IsActive { get; set; }
        [EpplusDisplay("Trạng thái")]
        public string StateDisplay
        {
            get
            {
                if (!this.IsActive)
                {
                    return "Ngừng điều trị";
                }
                else
                {
                    return this.State == "sale" ? "Đang điều trị" : "Hoàn thành";
                }
            }
            set { }
        }
        [EpplusDisplay("Phiếu điều trị")]
        public string OrderName { get; set; }

    }

    public class ServiceSaleReportPrint
    {
        public IEnumerable<SaleOrderLineBasic> data { get; set; } = new List<SaleOrderLineBasic>();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
    }
}
