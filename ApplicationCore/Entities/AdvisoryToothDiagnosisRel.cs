using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AdvisoryToothDiagnosisRel
    {
        public Guid AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }

        public Guid ToothDiagnosisId { get; set; }
        public ToothDiagnosis ToothDiagnosis { get; set; }
    }
}
