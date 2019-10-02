using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrderLine: BaseEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
        public Partner Customer { get; set; }

        /// <summary>
        /// Tên labo
        /// </summary>
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid SupplierId { get; set; }
        public Partner Supplier { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal PriceSubtotal { get; set; }

        /// <summary>
        /// Ngày gửi
        /// </summary>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// Ngày nhận
        /// </summary>
        public DateTime? ReceivedDate { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string Note { get; set; }

        public Guid? InvoiceId { get; set; }
        public AccountInvoice Invoice { get; set; }

        public Guid? DotKhamId { get; set; }
        public DotKham DotKham { get; set; }
    }
}
