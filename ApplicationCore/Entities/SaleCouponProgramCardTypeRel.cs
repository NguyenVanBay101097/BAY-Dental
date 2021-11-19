using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgramCardTypeRel
    {
        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }
        public Guid CardTypeId { get; set; }
        public CardType CardType { get; set; }
    }
}
