using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepSave
    {
        public Guid? DotKhamId { get; set; }
        public string State { get; set; }
        public bool IsDone { get; set; }
        public Guid OrderLineId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public bool Default { get; set; }
    }
}
