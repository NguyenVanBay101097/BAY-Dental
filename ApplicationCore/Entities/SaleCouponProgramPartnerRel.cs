using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgramPartnerRel
    {
        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
