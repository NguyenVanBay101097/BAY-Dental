using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ToothDiagnosis : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<AdvisoryToothDiagnosisRel> AdvisoryToothDiagnosisRels { get; set; } = new List<AdvisoryToothDiagnosisRel>();
    }
}
