using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceLineDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public Guid? UoMId { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid AccountId { get; set; }

        public decimal PriceUnit { get; set; }
        public decimal PriceSubTotal { get; set; }
        public decimal Quantity { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }

        public Guid? ToothId { get; set; }
        public ToothBasic Tooth { get; set; }

        public Guid? InvoiceId { get; set; }
        public AccountInvoiceSimple Invoice { get; set; }

        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
    }
}
