using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ToothDiagnosis : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ToothDiagnosisProductRel> ToothDiagnosisProductRels { get; set; } = new List<ToothDiagnosisProductRel>();
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
