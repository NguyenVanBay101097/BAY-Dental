using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// người giới thiệu
    /// </summary>
    public class Agent : BaseEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// customer : khách hàng
        /// employee : nhân viên
        /// partner : đối tác
        /// </summary>
        public string Classify { get; set; }

        public Guid? CustomerId { get; set; }
        public Partner Customer { get; set; }

        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

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
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// ngân hàng
        /// </summary>
        public Guid? BankId { get; set; }
        public ResBank Bank { get; set; }

        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        public string BankBranch { get; set; }

        /// <summary>
        /// Số tài khoản
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Chủ tài khoản
        /// </summary>
        public string AccountHolder { get; set; }


        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// kết nối bảng hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }
        public Commission Commission { get; set; }

        /// <summary>
        /// danh sách khách hàng được giơi thiệu
        /// </summary>
        public ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}
