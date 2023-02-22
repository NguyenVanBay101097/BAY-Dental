using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ToothDiagnosisProductRel
    {
        public Guid ToothDiagnosisId { get; set; }
        public ToothDiagnosis ToothDiagnosis { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
