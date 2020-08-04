using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionProductRuleBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal PercentFixed { get; set; }
        public string AppliedOn { get; set; }
    }
}
