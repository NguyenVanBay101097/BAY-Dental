using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderSave
    {
        public LaboOrderSave()
        {
            DateOrder = DateTime.Now;
            State = "draft";
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


        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Ngày gửi
        /// </summary>
        public DateTime DateOrder { get; set; }

       
        /// <summary>
        /// Ngày nhận dự kiến
        /// </summary>
        public DateTime? DatePlanned { get; set; }

        /// <summary>
        /// Ngày nhận thực tế
        /// </summary>
        public DateTime? DateReceipt { get; set; }

        /// <summary>
        /// Ngày xuất labo 
        /// 7 kêu trường này đặt tên như vậy
        /// </summary>
        public DateTime? DateExport { get; set; }

        public Guid CompanyId { get; set; }

        /// <summary>
        /// vật liệu
        /// </summary>
        public Guid? ProductId { get; set; }

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

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal AmountTotal { get; set; }

        public Guid? SaleOrderLineId { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        /// <summary>
        /// draft: Nháp
        /// confirmed: Đã xác nhận
        /// </summary>
        public string State { get; set; }

        public Guid? AccountMoveId { get; set; }

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

        /// <summary>
        /// khớp cắn
        /// </summary>
        public Guid? LaboBiteJointId { get; set; }

        /// <summary>
        /// kiểu nhịp
        /// </summary>
        public Guid? LaboBridgeId { get; set; }

        /// <summary>
        /// list gửu kèm
        /// </summary>
        public IEnumerable<ProductSimple> AttechRels { get; set; } = new List<ProductSimple>();

        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
        /// <summary>
        /// list image for laboorder
        /// </summary>
        public IEnumerable<IrAttachmentSave> Images = new List<IrAttachmentSave>();
    }
}
