using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// dữ liệu lấy từ function:
    /// </summary>
    public class PartnerInfo
    {
        public Guid Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? Avatar { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string NameNoSign { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? BirthYear { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        /// <summary>
        /// trạng thái của điều trị: done(all phiếu done), sale(có 1 phiếu sale), draft(còn lại)
        /// </summary>
        public string OrderState { get; set; }
        /// <summary>
        /// số tiền dự kiến thu
        /// </summary>
        public decimal? OrderResidual { get; set; }
        /// <summary>
        /// công nợ
        /// </summary>
        public decimal? TotalDebit { get; set; }
        /// <summary>
        /// hạng thành viên
        /// </summary>
        public Guid? MemberLevelId { get; set; }
        /// <summary>
        /// hạng thành viên
        /// </summary>
        public MemberLevel MemberLevel { get; set; }
        /// <summary>
        /// list id nhãn khách hàng
        /// </summary>
        public string PartnerCategIds { get; set; }

    }
}
