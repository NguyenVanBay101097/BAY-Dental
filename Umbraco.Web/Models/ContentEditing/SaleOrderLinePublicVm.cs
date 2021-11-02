using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLinePublicFilter
    {
        public Guid SaleOrderId { get; set; }
    }

    public class SaleOrderLinePublic
    {
        public string ProductName { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public DateTime? Date { get; set; }

        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manual :  thủ công
        /// </summary>
        public string ToothType { get; set; }

        /// <summary>
        /// rang
        /// </summary>
        public IEnumerable<string> Teeth { get; set; } = new List<string>();

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
                        return string.Join(", ", Teeth);
                }
            }
        }

        /// <summary>
        /// Chẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public string DoctorName { get; set; }

        public string AssistantName { get; set; }

        public string CounselorName { get; set; }

        public decimal PriceTotal { get; set; }

        public decimal? AmountInvoiced { get; set; }

        public decimal? AmountResidual
        {
            get
            {
                return PriceTotal - AmountInvoiced;
            }
        }

        /// <summary>
        /// Tổng đơn giá giảm của dịch vụ
        /// </summary>
        public double? PriceDiscountTotal { get; set; }

        public string State { get; set; }
    }
}
