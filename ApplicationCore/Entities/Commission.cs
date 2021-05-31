using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Commission : BaseEntity
    {
        public Commission()
        {
            Active = true;
        }

        public string Name { get; set; }

        public bool Active { get; set; }

        public ICollection<CommissionProductRule> CommissionProductRules { get; set; } = new List<CommissionProductRule>();

        /// <summary>
        /// doctor : hoa hồng bác sĩ
        /// assistant : hoa hồng phụ tá
        /// counselor : hoa hồng tư vấn
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

    }
}
