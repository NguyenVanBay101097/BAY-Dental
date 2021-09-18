using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class PurchaseOrderPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }
        public string Name { get; set; }

        public DateTime DateOrder { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public string PartnerName { get; set; }

        public string UserName { get; set; }

        public string StockPickingName { get; set; }

        public IEnumerable<PurchaseOrderLinePrintTemplate> OrderLines { get; set; } = new List<PurchaseOrderLinePrintTemplate>();

        public decimal AmountTotal { get; set; }

        public string CreatedById { get; set; }
    }

    public class PurchaseOrderLinePrintTemplate
    {
        public string Name { get; set; }
        public int? Sequence { get; set; }
        public decimal ProductQty { get; set; }
        public string ProductUOMName { get; set; }
        public decimal PriceSubtotal { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal Discount { get; set; }
    }
}
