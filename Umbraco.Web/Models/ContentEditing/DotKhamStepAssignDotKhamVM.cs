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

    public class DotKhamStepSetDone
    {
        public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
        public Guid DotKhamId { get; set; }
        public bool IsDone { get; set; }
    }
}
