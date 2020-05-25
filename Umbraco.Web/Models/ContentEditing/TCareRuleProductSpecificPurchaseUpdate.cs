using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareRuleProductSpecificPurchaseUpdate
    {
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
    }
}
