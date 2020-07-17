using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountAccount: BaseEntity
    {
        public AccountAccount()
        {
            Active = true;
        }

        /// <summary>
        /// Tên
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Code { get; set; }

        public Guid UserTypeId { get; set; }
        public AccountAccountType UserType { get; set; }

        public bool Active { get; set; }

        public string Note { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        //UserType.Type
        public string InternalType { get; set; }

        public bool Reconcile { get; set; }

        /// <summary>
        /// Co khong hien thi trong bao cao ket qua kinh doanh?
        /// </summary>
        public bool IsExcludedProfitAndLossReport { get; set; }
    }
}
