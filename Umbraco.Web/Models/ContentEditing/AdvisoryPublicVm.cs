using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AdvisoryPublicFilter
    {
        public Guid PartnerId { get; set; }
    }

    public class AdvisoryPublicReponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public EmployeeSimple Employee { get; set; }
        public string ToothType { get; set; }
        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductSimplePublic> Product { get; set; } = new List<ProductSimplePublic>();

        public string Note { get; set; }
    }
}
