using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FacebookMassMessagingTagStatistics
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public IEnumerable<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
