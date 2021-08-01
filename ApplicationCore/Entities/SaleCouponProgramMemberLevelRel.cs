using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgramMemberLevelRel
    {
        public Guid ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }
        public Guid MemberLevelId { get; set; }
        public MemberLevel MemberLevel { get; set; }
    }
}
