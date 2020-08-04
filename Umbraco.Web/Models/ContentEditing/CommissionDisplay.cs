using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public IEnumerable<CommissionProductRuleDisplay> CommissionProductRules { get; set; } = new List<CommissionProductRuleDisplay>();

    }
}
