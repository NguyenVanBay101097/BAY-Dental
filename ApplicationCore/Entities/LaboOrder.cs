using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrder: BaseEntity
    {
        public LaboOrder()
        {
            DateOrder = DateTime.Now;
        }

        public string Name { get; set; }

        /// <summary>
        /// Vendor Reference
        /// </summary>
        //public string PartnerRef { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        //public Guid? CustomerId { get; set; }
        //public Partner Customer { get; set; }

        /// <summary>
        /// Ngày gửi
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Ngày nhận
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<LaboOrderLine> OrderLines { get; set; } = new List<LaboOrderLine>();

        //public Guid? DotKhamId { get; set; }
        //public DotKham DotKham { get; set; }

        //public string UserId { get; set; }
        //public ApplicationUser User { get; set; }

        //public Guid? SaleOrderId { get; set; }
        //public SaleOrder SaleOrder { get; set; }
        /// <summary>
        /// labo nào : lấy từ product type labo
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// màu sắc
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// đơn giá
        /// </summary>
        public decimal PriceUnit { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }
        /// <summary>
        /// list răng
        /// </summary>
        public ICollection<LaboOrderToothRel> LaboOrderToothRel { get; set; } = new List<LaboOrderToothRel>();
    }
}
