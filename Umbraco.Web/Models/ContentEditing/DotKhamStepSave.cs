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
    }
}
