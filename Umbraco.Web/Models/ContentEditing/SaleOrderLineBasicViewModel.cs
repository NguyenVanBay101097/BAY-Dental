using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineBasicViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Diagnostic { get; set; }
        public Guid? ProductId { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
        public IEnumerable<DotKhamStepBasic> Steps { get; set; } = new List<DotKhamStepBasic>();
    }
}
