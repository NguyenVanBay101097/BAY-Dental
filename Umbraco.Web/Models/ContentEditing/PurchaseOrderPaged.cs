using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderPaged
    {
        public PurchaseOrderPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public string Type { get; set; }

        public DateTime? DateOrderFrom { get; set; }

        public DateTime? DateOrderTo { get; set; }

        public string State { get; set; }
    }
}
