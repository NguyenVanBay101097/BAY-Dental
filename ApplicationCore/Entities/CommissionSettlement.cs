using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CommissionSettlement : BaseEntity
    {

        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Tiền thanh toán trên 1 line
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
        /// tiền hoa hồng 
        /// </summary>
        public decimal? Amount { get; set; }

        public Guid? MoveLineId { get; set; }
        public AccountMoveLine MoveLine { get; set; }

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

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

    }
}
