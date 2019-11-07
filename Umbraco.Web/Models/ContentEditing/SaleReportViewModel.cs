using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportTopServicesFilter
    {
        public SaleReportTopServicesFilter()
        {
            Number = 5;
        }
        public int Number { get; set; }

        public bool ByInvoice { get; set; }

        public bool ByQty { get; set; }

        public Guid? CompanyId { get; set; }

        public Guid? PartnerId { get; set; }

        public string State { get; set; }

        public Guid? CategId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class SaleReportTopServicesCs
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        
        public decimal PriceTotalSum { get; set; }

        public decimal? ProductUOMQtySum { get; set; }
    }
}
