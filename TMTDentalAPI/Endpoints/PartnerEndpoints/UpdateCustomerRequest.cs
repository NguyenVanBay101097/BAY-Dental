using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints
{
    public class UpdateCustomerRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        public string Gender { get; set; }

        public string Street { get; set; }

        public int? BirthYear { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

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

        /// <summary>
        /// Danh xưng
        /// </summary>
        public Guid? TitleId { get; set; }

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

        public IEnumerable<Guid> HistoryIds { get; set; } = new List<Guid>();

        public DateTime? Date { get; set; }

        public Guid? AgentId { get; set; }
    }
}
