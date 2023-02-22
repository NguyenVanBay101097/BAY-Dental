using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderLinePrintVM
    {
        public string ProductName { get; set; }

        public decimal ProductQty { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal PriceSubtotal { get; set; }

        public decimal PriceTotal { get; set; }

        public int? Sequence { get; set; }
    }
}
