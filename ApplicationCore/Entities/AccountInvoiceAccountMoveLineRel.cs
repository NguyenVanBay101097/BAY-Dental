using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountInvoiceAccountMoveLineRel
    {
        public Guid AccountInvoiceId { get; set; }
        public AccountInvoice AccountInvoice { get; set; }
        public Guid MoveLineId { get; set; }
        public AccountMoveLine MoveLine { get; set; }
    }
}
