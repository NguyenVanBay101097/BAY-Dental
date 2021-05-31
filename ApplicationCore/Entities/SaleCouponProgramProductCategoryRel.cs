using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgramProductCategoryRel
    {
        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }

        public Guid ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
