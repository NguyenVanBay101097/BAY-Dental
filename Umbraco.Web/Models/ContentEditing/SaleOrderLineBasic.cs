using System;
using System.Collections.Generic;
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

        public Guid OrderId { get; set; }
        public SaleOrderBasic Order { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public string Diagnostic { get; set; }

        public DateTime DateCreated { get; set; }

        public EmployeeBasic Employee { get; set; }

        public EmployeeBasic Assistant { get; set; }
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

        public decimal? AmountResidual
        {
            get
            {
                return PriceTotal - AmountPaid;
            }
        }

        public bool IsActive { get; set; }
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
        [EpplusDisplay("Răng")]
        public string Teeth { get; set; }
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
}
