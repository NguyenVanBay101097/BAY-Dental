using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceLinePrint
    {
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal Discount { get; set; }

        public decimal PriceSubtotal { get; set; }

        public int? Sequence { get; set; }
    }
}
