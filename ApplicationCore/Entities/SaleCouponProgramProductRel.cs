using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgramProductRel
    {
        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
