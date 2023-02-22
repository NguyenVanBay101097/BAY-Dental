using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ApplicationCore.Models
{
    public class SimpleFilter
    {
        public string type { get; set; }

        public IEnumerable<SimpleFilterItem> items { get; set; } = new List<SimpleFilterItem>();
    }

    public class SimpleFilterItem
    {
        public string type { get; set; }
        public string name { get; set; }
        public string formula_type { get; set; }
        public string formula_value { get; set; }
        public string formula_display { get; set; }
    }
}
