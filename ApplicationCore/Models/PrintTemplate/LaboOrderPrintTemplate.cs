using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class LaboOrderPrintTemplate
    {
        public string Name { get; set; }

        public CompanyPrintTemplate Company { get; set; }

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerPrintTemplate Partner { get; set; }

        public Guid CustomerId { get; set; }
        public PartnerPrintTemplate Customer { get; set; }

        public DateTime DateOrder { get; set; }

        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Ngày nhận (dự kiến)
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        /// <summary>
        /// Vật liệu
        /// </summary>
        public Guid? ProductId { get; set; }
        public ProductSimplePrintTemplate Product { get; set; }

        /// <summary>
        /// màu sắc
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// chỉ định
        /// </summary>
        public string Indicated { get; set; }

        /// <summary>
        /// ghi chú kỹ thuật
        /// </summary>
        public string TechnicalNote { get; set; }

        /// <summary>
        /// đường hoàn tất
        /// </summary>
        public Guid? LaboFinishLineId { get; set; }
        public LaboFinishLineSimpleTemplate LaboFinishLine { get; set; }

        /// <summary>
        /// khớp cắn
        /// </summary>
        public Guid? LaboBiteJointId { get; set; }
        public LaboBiteJointSimplePrintTemplate LaboBiteJoint { get; set; }

        /// <summary>
        /// kiểu nhịp
        /// </summary>
        public Guid? LaboBridgeId { get; set; }
        public LaboBridgeSimplePrintTemplate LaboBridge { get; set; }

        /// <summary>
        /// list gửu kèm
        /// </summary>
        public IEnumerable<ProductSimplePrintTemplate> LaboOrderProducts { get; set; } = new List<ProductSimplePrintTemplate>();

        /// <summary>
        /// số lượng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// đơn giá
        /// </summary>
        public decimal PriceUnit { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLinePrintTemplate SaleOrderLine { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        public DateTime? WarrantyPeriod { get; set; }

        public IEnumerable<ToothSimplePrintTemplate> Teeth { get; set; } = new List<ToothSimplePrintTemplate>();
    }

    public class LaboFinishLineSimpleTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LaboBiteJointSimplePrintTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class LaboBridgeSimplePrintTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

}
