using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResPartnerBank : BaseEntity
    {
        /// <summary>
        /// tên chủ tài khoản
        /// </summary>
        public string AccountHolderName { get; set; }

        public string AccountNumber { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid BankId { get; set; }
        public ResBank Bank { get; set; }

        public int Sequence { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        /// <summary>
        /// chi nhánh ngân hàng
        /// </summary>
        public string Branch { get; set; }

    }
}
