using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrJob : BaseEntity
    {
        /// <summary>
        /// Tên chức vụ
        /// </summary>
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
