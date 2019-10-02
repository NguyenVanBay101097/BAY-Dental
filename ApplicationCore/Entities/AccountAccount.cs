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

        public string Name { get; set; }

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
    }
}
