using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class MedicineOrderPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime OrderDate { get; set; }

        public EmployeePrintTemplate Employee { get; set; }

        public ToaThuocPrintTemplate ToaThuoc { get; set; }

        public PartnerPrintTemplate Partner { get; set; }

        public AccountPaymentBasicPrintTemplate AccountPayment { get; set; }

        public decimal Amount { get; set; }
        public string State { get; set; }

        public IEnumerable<MedicineOrderLinePrintTemplate> MedicineOrderLines { get; set; } = new List<MedicineOrderLinePrintTemplate>();
    }

    public class AccountPaymentBasicPrintTemplate
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

    public class MedicineOrderLinePrintTemplate
    {
        public ToaThuocLinePrintTemplate ToaThuocLine { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal AmountTotal { get; set; }
        public ProductSimplePrintTemplate Product { get; set; }
        public UoMBasicPrintTemplate ProductUoM { get; set; }
    }
}
