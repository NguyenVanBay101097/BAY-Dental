using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class QuotationPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }
        public PartnerPrintTemplate Partner { get; set; }
        public string Note { get; set; }
        public DateTime DateQuotation { get; set; }
        public EmployeePrintTemplate Employee { get; set; }
        public int DateApplies { get; set; }
        public string Name { get; set; }
        public DateTime? DateEndQuotation { get; set; }
        public IEnumerable<QuotationLinePrintTemplate> Lines { get; set; } = new List<QuotationLinePrintTemplate>();
        public decimal? TotalAmount { get; set; }
        public IEnumerable<PaymentQuotationPrintTemplate> Payments { get; set; } = new List<PaymentQuotationPrintTemplate>();
    }

    public class QuotationLinePrintTemplate
    {
        public string Name { get; set; }

        /// <summary>
        /// Dịch vụ
        /// </summary>
        public ProductSimplePrintTemplate Product { get; set; }

        /// <summary>
        /// Số lượng dịch vụ
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Giảm giá theo %
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Tổng tiền trên 1 dịch vụ
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Tien cua 1 dich vu
        /// </summary>
        public decimal? SubPrice { get; set; }

        /// <summary>
        /// tổng đơn giá ưu đãi của dịch vụ
        /// </summary>
        public decimal? AmountDiscountTotal { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }    

        public EmployeePrintTemplate Employee { get; set; }

        public string ToothType { get; set; }


    }

    public class PaymentQuotationPrintTemplate
    {
        /// <summary>
        /// Số thứ tự thanh toán
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Phần trăm / Tiền mặt
        /// </summary>
        public string DiscountPercentType { get; set; }

        /// <summary>
        /// Số tiền thanh toán / Sô %
        /// </summary>
        public decimal? Payment { get; set; }

        /// <summary>
        /// Tổng số tiền trên 1 lần thanh toán
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime? Date { get; set; }

    }
}
