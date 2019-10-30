using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepAssignDotKhamVM
    {
        public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
        public Guid DotKhamId { get; set; }
    }
}
