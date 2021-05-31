using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToothDiagnosisBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ToothDiagnosisPaged
    {
        public ToothDiagnosisPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }

    public class ToothDiagnosisSave
    {
        public string Name { get; set; }
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
    }

    public class ToothDiagnosisDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProductSimple> Product { get; set; } = new List<ProductSimple>();
    }

  
}
