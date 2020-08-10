using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SetupChamcong : BaseEntity
    {
        /// <summary>
        /// thời gian chuẩn 1 công: set giá trị này cho chấm công khi status là nghỉ phép
        /// </summary>
        public decimal StandardWorkHour { get; set; }
        /// <summary>
        /// thời gian chuẩn 1 công cho phép từ mấy tiếng
        /// </summary>
        public decimal OneStandardWorkFrom { get; set; }
        /// <summary>
        /// thời gian chuẩn 1 công cho phép đến mấy tiếng
        /// </summary>
        public decimal OneStandardWorkTo { get; set; }
        /// <summary>
        /// thời gian chuẩn nửa công từ mấy tiếng
        /// </summary>
        public decimal HalfStandardWorkFrom { get; set; }
        /// <summary>
        /// thời gian chuẩn nửa công đến mấy tiếng
        /// </summary>
        public decimal HalfStandardWorkTo { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
