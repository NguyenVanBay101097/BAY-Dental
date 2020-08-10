using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SetupChamcong : BaseEntity
    {
        /// <summary>
        /// thời gian chuẩn cho 1 công
        /// </summary>
        public decimal OneStandardWorkHour { get; set; }
        /// <summary>
        /// thời gian chuẩn cho nửa công
        /// </summary>
        public decimal HalfStandardWorkHour { get; set; }
        /// <summary>
        /// thời gian chênh lệch cho phép
        /// </summary>
        public decimal DifferenceTime { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
