using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsDone { get; set; }
    }
}
