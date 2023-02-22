using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class IRRuleBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class IRRuleNew
    {
        public string NameRule { get; set; }
        public string ModelIRModel { get; set; }
        public string NameIRModel { get; set; }
    }
}
