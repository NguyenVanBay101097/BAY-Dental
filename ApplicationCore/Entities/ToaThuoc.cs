using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ToaThuoc: BaseEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Lời dặn
        /// </summary>
        public string Note { get; set; }

          /// <summary>
          /// Chẩn đoán
          /// </summary>
        public string Diagnostic { get; set; }

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

        public ICollection<ToaThuocLine> Lines { get; set; } = new List<ToaThuocLine>();
    }
}
