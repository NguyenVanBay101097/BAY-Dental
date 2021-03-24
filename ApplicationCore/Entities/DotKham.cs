using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Tien trinh dieu tri
    /// </summary>
    public class DotKham : BaseEntity
    {
        public DotKham()
        {
            State = "draft";
            Date = DateTime.Now;
        }

        public int? Sequence { get; set; }

        /// <summary>
        /// Mã đợt khám
        /// </summary>
        public string Name { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        /// <summary>
        /// Khách hàng lấy từ SaleOrder
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// confirmed: Đã xác nhận
        /// cancel: Hủy bỏ
        /// </summary>
        public string State { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public Employee Doctor { get; set; }

        public Guid? AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public ICollection<DotKhamLine> Lines { get; set; } = new List<DotKhamLine>();

        /// <summary>
        /// hình ảnh
        /// </summary>
        public ICollection<PartnerImage> DotKhamImages { get; set; } = new List<PartnerImage>();

        /// <summary>
        /// phụ tá
        /// </summary>
        public Guid? AssistantId { get; set; }
        public Employee Assistant { get; set; }
    }
}
