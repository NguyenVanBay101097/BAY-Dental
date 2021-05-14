using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Nhân viên
    /// </summary>
    public class Employee: BaseEntity
    {
        public Employee()
        {
            Active = true;
            IsDoctor = false;
            IsAssistant = false;
        }

        public string Name { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// CMND
        /// </summary>
        public string IdentityCard { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        public Guid? CategoryId { get; set; }
        public EmployeeCategory Category { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Là bác sĩ
        /// </summary>
        public bool IsDoctor { get; set; }

        /// <summary>
        /// Là phụ tá
        /// </summary>
        public bool IsAssistant { get; set; }

        /// <summary>
        /// kết nối hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }
        public Commission Commission { get; set; }

        /// <summary>
        /// kết nối hoa hồng
        /// </summary>
        public Guid? AssistantCommissionId { get; set; }
        public Commission AssistantCommission { get; set; }

        /// <summary>
        /// kết nối hoa hồng
        /// </summary>
        public Guid? CounselorCommissionId { get; set; }
        public Commission CounselorCommission { get; set; }

        /// <summary>
        /// kết nối người dùng
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ChamCong> ChamCongs { get; set; } = new List<ChamCong>();

        public Guid? StructureTypeId { get; set; }
        public HrPayrollStructureType StructureType { get; set; }

        /// <summary>
        /// Tiền lương theo tháng, lương cơ bản theo hợp đồng
        /// </summary>
        public decimal? Wage { get; set; }

        /// <summary>
        /// Tiền lương mỗi giờ, nếu StructureType.WageType == hourly
        /// </summary>
        public decimal? HourlyWage { get; set; }

        /// <summary>
        ///  ngày bắt đầu đi làm của nhân viên
        /// </summary>

        public DateTime? StartWorkDate { get; set; }
        
        /// <summary>
        /// Mã vân tay
        /// </summary>
        public string EnrollNumber { get; set; }

        /// <summary>
        /// Số ngày nghỉ/tháng
        /// </summary>
        public decimal? LeavePerMonth { get; set; }

        /// <summary>
        /// số giờ công chuẩn 1 ngày
        /// </summary>
        public decimal? RegularHour { get; set; }

        /// <summary>
        /// tỉ lệ lương tăng ca
        /// </summary>
        public decimal? OvertimeRate { get; set; }
        /// <summary>
        /// tỉ lệ lương ngày đi làm thêm
        /// </summary>
        public decimal? RestDayRate { get; set; }
        /// <summary>
        /// trợ cấp
        /// </summary>
        public decimal? Allowance { get; set; }
        public string Avatar { get; set; }
        /// <summary>
        /// được phép làm khảo sát hay không
        /// </summary>
        public bool IsAllowSurvey { get; set; }
        /// <summary>
        /// để phân biệt chức vụ trong survey: quản lý khảo sát hay là nhân viên khảo sát
        /// </summary>
        public ResGroup Group { get; set; }
        public Guid? GroupId { get; set; }
    }
}
