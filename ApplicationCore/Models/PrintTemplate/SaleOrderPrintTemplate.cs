using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class SaleOrderPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }

        public ApplicationUserPrintTemplate User { get; set; }

        public PartnerPrintTemplate Partner { get; set; }

        public string Name { get; set; }
        public DateTime DateOrder { get; set; }

        public IEnumerable<SaleOrderLinePrintTemplate> OrderLines { get; set; } = new List<SaleOrderLinePrintTemplate>();

        public IEnumerable<SaleOrderPaymentBasicPrintTemplate> HistoryPayments { get; set; } = new List<SaleOrderPaymentBasicPrintTemplate>();

        public IEnumerable<DotKhamPrintTemplate> DotKhams { get; set; } = new List<DotKhamPrintTemplate>();

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Tổng thanh toán
        /// </summary>
        public decimal TotalPaid { get; set; }

        public decimal Residual { get; set; }
    }

    public class SaleOrderLinePrintTemplate
    {
        public ProductSimplePrintTemplate Product { get; set; }

        public EmployeePrintTemplate Employee { get; set; }

        public string ProductName { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        public int? Sequence { get; set; }

        public double? AmountDiscountTotal { get; set; }
    }

    public class SaleOrderPaymentBasicPrintTemplate
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string Note { get; set; }

        public IEnumerable<AccountPaymentDisplayPrintTemplate> Payments { get; set; } = new List<AccountPaymentDisplayPrintTemplate>();

        /// <summary>
        /// draft : nháp
        /// posted : đã thanh toán
        /// cancel : hủy
        /// </summary>
        public string State { get; set; }
    }

    public class AccountPaymentDisplayPrintTemplate
    {
        public string PartnerName { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PartnerType { get; set; }

        public string PaymentType { get; set; }

        public string JournalName { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public string Communication { get; set; }

        public string DisplayPaymentType
        {
            get
            {
                switch (PaymentType)
                {
                    case "inbound":
                        return "Phiếu thu";
                    case "outbound":
                        return "Phiếu chi";
                    default:
                        return "";
                }
            }
        }

        public decimal AmountSigned
        {
            get
            {
                var sign = PaymentType == "outbound" ? -1 : 1;
                return Amount * sign;
            }
        }

        public string DisplayState
        {
            get
            {
                switch (State)
                {
                    case "posted":
                        return "Đã xác nhận";
                    case "cancel":
                        return "Đã hủy";
                    default:
                        return "Nháp";
                }
            }
        }
    }

    public class DotKhamPrintTemplate
    {
        public int? Sequence { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public EmployeePrintTemplate Doctor { get; set; }
        public EmployeePrintTemplate Assistant { get; set; }

        public IEnumerable<DotKhamLinePrintTemplate> Lines { get; set; } = new List<DotKhamLinePrintTemplate>();

        public string Name { get; set; }
    }

    public class DotKhamLinePrintTemplate
    {
        public string NameStep { get; set; }

        public ProductSimplePrintTemplate Product { get; set; }

        public string Note { get; set; }

        public IEnumerable<ToothSimplePrintTemplate> Teeth { get; set; } = new List<ToothSimplePrintTemplate>();
    }

    public class ToothSimplePrintTemplate
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

    }


}
