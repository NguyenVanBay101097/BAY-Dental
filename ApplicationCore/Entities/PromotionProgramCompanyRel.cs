using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PromotionProgramCompanyRel
    {
        public Guid PromotionProgramId { get; set; }
        public PromotionProgram PromotionProgram { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
