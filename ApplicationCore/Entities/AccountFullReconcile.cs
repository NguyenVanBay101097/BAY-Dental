using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountFullReconcile: BaseEntity
    {
        public string Name { get; set; }

        public ICollection<AccountPartialReconcile> PartialReconciles { get; set; }

        public ICollection<AccountMoveLine> ReconciledLines { get; set; }

        public Guid? ExchangeMoveId { get; set; }
        public virtual AccountMove ExchangeMove { get; set; }
    }
}
