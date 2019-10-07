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

        /// <summary>
        /// Là bác sĩ
        /// </summary>
        public bool IsDoctor { get; set; }

        /// <summary>
        /// Là phụ tá
        /// </summary>
        public bool IsAssistant { get; set; }
    }
}
