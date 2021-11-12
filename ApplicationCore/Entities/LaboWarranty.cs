using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboWarranty : BaseEntity
    {
        public LaboWarranty()
        {
            Name = "/";
            DateReceiptWarranty = DateTime.Now;
            State = "draft";
        }

        /// <summary>
        /// Phiếu bảo hành
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phiếu Labo
        /// </summary>
        public Guid? LaboOrderId { get; set; }
        public LaboOrder LaboOrder { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Ngày nhận bảo hành
        /// </summary>
        public DateTime? DateReceiptWarranty { get; set; }

        /// <summary>
        /// Ngày gửi bảo hành
        /// </summary>
        public DateTime? DateSendWarranty { get; set; }

        /// <summary>
        /// Ngày nhận nghiệm thu
        /// </summary>
        public DateTime? DateReceiptInspection { get; set; }

        /// <summary>
        /// Ngày lắp bảo hành
        /// </summary>
        public DateTime? DateAssemblyWarranty { get; set; }

        /// <summary>
        /// Danh sách răng
        /// </summary>
        public ICollection<LaboWarrantyToothRel> LaboWarrantyToothRels { get; set; } = new List<LaboWarrantyToothRel>();

        /// <summary>
        /// Lý do bảo hành
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Nội dung bảo hành
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Nháp
        /// new: Mới
        /// sent: Đã gửi
        /// received: Đã nhận
        /// assembled: Đã lắp
        /// </summary>
        public string State { get; set; }
    }
}
