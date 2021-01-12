using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ToaThuoc: BaseEntity
    {
        public ToaThuoc()
        {
            Date = DateTime.Now;
        }

        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Ngày tái khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Lời dặn
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Liên kết với đợt khám nào?
        /// </summary>
        public Guid? DotKhamId { get; set; }
        public DotKham DotKham { get; set; }

        /// <summary>
        /// Tài khoản tạo toa thuốc này
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ICollection<ToaThuocLine> Lines { get; set; } = new List<ToaThuocLine>();
        /// <summary>
        /// liên kết với 1 điều trị
        /// </summary>
        public Guid? SaleOrderID { get; set; }
        public SaleOrder SaleOrder { get; set; }

        public DateTime? ReExaminationDate { get; set; }

        public string Diagnostic { get; set; }

        public ICollection<MedicineOrder> MedicineOrders { get; set; } = new List<MedicineOrder>();
    }
}
