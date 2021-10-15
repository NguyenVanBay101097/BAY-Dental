using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLinePublicFilter
    {
        public Guid SaleOrderId { get; set; }
    }

    public class SaleOrderLinePublic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// don gia
        /// </summary>
        public decimal PriceUnit { get; set; }

        /// <summary>
        /// so luong
        /// </summary>
        public decimal Quantity { get; set; }

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
        public IEnumerable<ToothSimple> Teeth { get; set; } = new List<ToothSimple>();

        /// <summary>
        /// Chẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }


        /// <summary>
        /// Bác sĩ
        /// </summary>
        public EmployeeSimple Doctor { get; set; }

        /// <summary>
        /// Phụ tá
        /// </summary>
        public EmployeeSimple Assistant { get; set; }

        /// <summary>
        /// người tư vấn
        /// </summary>
        public EmployeeSimple Counselor { get; set; }    

        /// <summary>
        /// Tong tien
        /// </summary>
        public decimal PriceSubTotal { get; set; }

        /// <summary>
        /// Số tiền đã thanh toán
        /// </summary>
        public decimal? AmountPaid { get; set; }

        /// <summary>
        /// Số tiền con lai
        /// </summary>
        public decimal? AmountResidual { get; set; }

        /// <summary>
        /// Tổng đơn giá giảm của dịch vụ
        /// </summary>
        public double? AmountDiscountTotal { get; set; }

        public string State { get; set; }

    }
}
