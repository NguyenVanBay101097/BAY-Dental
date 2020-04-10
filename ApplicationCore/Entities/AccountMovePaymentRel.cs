using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountMovePaymentRel
    {
        public Guid MoveId { get; set; }
        public AccountMove Move { get; set; }

        public Guid PaymentId { get; set; }
        public AccountPayment Payment { get; set; }
    }
}
