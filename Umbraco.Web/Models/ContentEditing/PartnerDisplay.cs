using System;
using System.Collections.Generic;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerDisplay: PartnerBasic
    {
        public PartnerDisplay()
        {
            Customer = true;
            Active = true;
        }

        /// <summary>
        /// Số nhà, đường
        /// </summary>
        public string Street { get; set; }

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
        /// Giới tính
        /// ('male', 'Male')
        /// ('female', 'Female')
        /// ('other', 'Other')
        /// </summary>
        public string Gender { get; set; }

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
        public string Source { get; set; }

        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employees { get; set; }

        /// <summary>
        /// Ghi chú khi nguồn là 'Khác'
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Ảnh chân dung
        /// </summary>
        public string Avatar { get; set; }

        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();


    }
}
