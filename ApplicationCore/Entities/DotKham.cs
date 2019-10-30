using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Tien trinh dieu tri
    /// </summary>
    public class DotKham: BaseEntity
    {
        public DotKham()
        {
            State = "draft";
            Date = DateTime.Now;
        }

        /// <summary>
        /// Ma tien trinh
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Hóa đơn điều trị
        /// </summary>
        public Guid? InvoiceId { get; set; }
        public AccountInvoice Invoice { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        /// <summary>
        /// Khách hàng lấy từ invoice
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Nhân viên
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// confirmed: Đã xác nhận
        /// cancel: Hủy bỏ
        /// </summary>
        public string State { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? DoctorId { get; set; }
        public Employee Doctor { get; set; }

        public Guid? AssistantId { get; set; }
        public Employee Assistant { get; set; }

        public Guid? AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public ICollection<DotKhamLine> Lines { get; set; } = new List<DotKhamLine>();
    }
}
