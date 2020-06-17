using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Khách hàng
    /// </summary>
    public class Partner : BaseEntity
    {
        public Partner()
        {
            Customer = true;
            Active = true;
        }

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string NameNoSign { get; set; }

        /// <summary>
        /// Số nhà, đường
        /// </summary>
        public string Street { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Check this box if this contact is a vendor. It can be selected in purchase orders.
        /// </summary>
        public bool Supplier { get; set; }

        /// <summary>
        /// Check this box if this contact is a customer. It can be selected in sales orders.
        /// </summary>
        public bool Customer { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Khả dụng
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Check this box if this contact is an Employee.
        /// </summary>
        public bool Employee { get; set; }

        /// <summary>
        /// Giới tính
        /// ('male', 'Male')
        /// ('female', 'Female')
        /// ('other', 'Other')
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Năm sinh
        /// </summary>
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
        /// Tiền sử bệnh khác, nên hiển thị nếu là bệnh nhân (khách hàng)
        /// </summary>
        public string MedicalHistory { get; set; }

        public ICollection<PartnerHistoryRel> PartnerHistoryRels { get; set; } = new List<PartnerHistoryRel>();

        /// <summary>
        /// Mã tỉnh/thành phố
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// Tên tỉnh/thành phố
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Mã quận/huyện
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// Tên quận/huyện
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// Mã phường xã
        /// </summary>
        public string WardCode { get; set; }

        /// <summary>
        /// Tên phường xã
        /// </summary>
        public string WardName { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string Barcode { get; set; }


        public ICollection<AccountMoveLine> AMoveLines { get; set; }

        public ICollection<PartnerPartnerCategoryRel> PartnerPartnerCategoryRels { get; set; } = new List<PartnerPartnerCategoryRel>();

        public string Fax { get; set; }

        /// <summary>
        /// Nguồn biết đến
        /// </summary>       
        public Guid? SourceId { get; set; }

        public PartnerSource Source { get; set; }

        /// <summary>
        /// Nhân viên giới thiệu
        /// </summary>
        public string ReferralUserId { get; set; }
        public ApplicationUser ReferralUser { get; set; }

        /// <summary>
        /// Ghi chú khi nguồn là 'Khác'
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ảnh chân dung
        /// </summary>
        public string Avatar { get; set; }

        public string ZaloId { get; set; }
    }
}
