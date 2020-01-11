using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderNoCodePromoProgram
    {
        public Guid OrderId { get; set; }
        public SaleOrder Order { get; set; }

        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }
    }
}
