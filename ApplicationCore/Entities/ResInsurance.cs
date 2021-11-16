using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// bảo hiểm
    /// </summary>
    public class ResInsurance
    {
        public ResInsurance()
        {
            IsActive = true;
        }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// người đại diện
        /// </summary>
        public string Representative { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public bool IsActive { get; set; }
    }
}
