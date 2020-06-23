using ApplicationCore.Entities;
using System;
using System.Collections.Generic;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerDisplay
    {
        public PartnerDisplay()
        {
            Customer = true;
            Active = true;
            Gender = "male";
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        public string Gender { get; set; }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthYear { get; set; }

        /// <summary>
        /// Là nhà cung cấp
        /// </summary>
        public bool Supplier { get; set; }

        /// <summary>
        /// Là khách hàng
        /// </summary>
        public bool Customer { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Khả dụng
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Là nhân viên
        /// </summary>
        public bool Employee { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }

        /// <summary>
        /// Tiền sử bệnh, nên hiển thị nếu là bệnh nhân (khách hàng)
        /// </summary>
        public string MedicalHistory { get; set; }

        /// <summary>
        /// Mã tỉnh/thành phố
        /// </summary>
        public CitySimple City { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public DistrictSimple District { get; set; }

        /// <summary>
        /// Tên phường xã
        /// </summary>
        public WardSimple Ward { get; set; }

        /// <summary>
        /// Nguồn biết đến
        /// </summary>
        public Guid? SourceId { get; set; }
        public PartnerSourceBasic Source { get; set; }

        /// <summary>
        /// Người giới thiệu
        /// </summary>
        public string ReferralUserId { get; set; }
        public ApplicationUserSimple ReferralUser { get; set; }

        /// <summary>
        /// Ghi chú khi nguồn là 'Khác'
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ảnh chân dung
        /// </summary>
        public string Avatar { get; set; }

        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();

        public IEnumerable<HistorySimple> Histories { get; set; } = new List<HistorySimple>();

        public string Fax { get; set; }

        public Guid? CompanyId { get; set; }

        public string ZaloId { get; set; }

        public DateTime? Date { get; set; }

        public string DisplayName { get; set; }
    }
}
