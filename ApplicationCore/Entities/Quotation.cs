using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Quotation : BaseEntity
    {
        /// <summary>
        /// Mã báo giá
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Partner Partner { get; set; }
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Người báo giá
        /// </summary>
        public Employee Employee { get; set; }
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Ngày báo giá
        /// </summary>
        public DateTime DateQuotation { get; set; }

        /// <summary>
        /// Ngày áp dụng
        /// </summary>
        public int DateApplies { get; set; }

        /// <summary>
        /// Ngày kết thúc báo giá
        /// </summary>
        public DateTime? DateEndQuotation { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tổng tiền báo giá
        /// </summary>
        public decimal? TotalAmount { get; set; }

        public ICollection<QuotationLine> Lines { get; set; } = new List<QuotationLine>();
        public string State { get; set; }

        public ICollection<PaymentQuotation> Payments { get; set; } = new List<PaymentQuotation>();
        public ICollection<SaleOrder> Orders { get; set; } = new List<SaleOrder>();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
