using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceLineOnChangeProduct
    {
        public string InvoiceType { get; set; }

        public Guid? ProductId { get; set; }
    }

    public class AccountInvoiceLineOnChangeProductResult
    {
        public Guid AccountId { get; set; }

        public decimal PriceUnit { get; set; }

        public Guid? UoMId { get; set; }

        public string Name { get; set; }
    }

    public class AccountInvoiceLineSimple
    {
        public string Name { get; set; }

        public string ProductId { get; set; }
    }
}
