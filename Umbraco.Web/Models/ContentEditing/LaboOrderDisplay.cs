using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderDisplay
    {
        public LaboOrderDisplay()
        {
            DateOrder = DateTime.Now;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public DateTime DateOrder { get; set; }

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Ngày nhận (dự kiến)
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

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
        public SaleOrderLineBasic SaleOrderLine { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        public DateTime? WarrantyPeriod { get; set; }

        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
        /// <summary>
        /// list image for laboorder
        /// </summary>
        public IEnumerable<IrAttachmentBasic> Images = new List<IrAttachmentBasic>();
    }
}
