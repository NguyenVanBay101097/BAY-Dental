using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Mapping
{
    public class ResourceCalendarPaged
    {
        public ResourceCalendarPaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }
}
