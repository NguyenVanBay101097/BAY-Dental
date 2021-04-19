using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CommissionSettlement : BaseEntity
    {
        //tam thoi co the chua can
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Tiền còn lai cần thanh toán trên 1 line
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Tiền lợi nhuận được tính từ tiền thanh toán
        /// </summary>
        public decimal? BaseAmount { get; set; }

        /// <summary>
        /// phần trăm hoa hồng de luu lai lich su
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// tiền hoa hồng bác sĩ
        /// </summary>
        public decimal? Amount { get; set; }

        public Guid? MoveLineId { get; set; }
        public AccountMoveLine MoveLine { get; set; }

        //Không cần
        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        //Không cần
        public Guid? PaymentId { get; set; }
        public AccountPayment Payment { get; set; }

        /// <summary>
        /// advisory : hoa hồng tư vấn
        /// doctor : hoa hồng bác sĩ
        /// assistant : hoa hồng phụ tá
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Bảng hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }
        public Commission Commission { get; set; }

        /// <summary>
        /// ngày tạo hoa hồng
        /// </summary>
        public DateTime? Date { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }
    }
}
