using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionPaged
    {
        public CommissionPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        public string Type { get; set; }
    }
}
