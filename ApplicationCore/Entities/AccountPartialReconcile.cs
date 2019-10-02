using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountPartialReconcile: BaseEntity
    {
        public Guid DebitMoveId { get; set; }
        public AccountMoveLine DebitMove { get; set; }

        public Guid CreditMoveId { get; set; }
        public AccountMoveLine CreditMove { get; set; }

        public decimal Amount { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? FullReconcileId { get; set; }
        public AccountFullReconcile FullReconcile { get; set; }
    }
}
