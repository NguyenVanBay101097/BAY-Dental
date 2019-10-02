using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Customer : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Số nhà, đường
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public string Gender { get; set; }

        public Guid? ProvinceId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? WardId { get; set; }
    }
}
