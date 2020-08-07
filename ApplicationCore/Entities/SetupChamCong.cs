using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SetupChamcong : BaseEntity
    {
        public decimal StandardWorkHour { get; set; }
        public decimal OneStandardWorkFrom { get; set; }
        public decimal OneStandardWorkTo { get; set; }
        public decimal HalfStandardWorkFrom { get; set; }
        public decimal HalfStandardWorkTo { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
