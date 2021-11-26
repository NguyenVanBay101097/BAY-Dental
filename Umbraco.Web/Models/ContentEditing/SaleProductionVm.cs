using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleProductionBasic
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public decimal Quantity { get; set; }
    }

    public class SaleProductionDisplay
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public ProductSimple Product { get; set; }

        public IEnumerable<SaleProductionLineDisplay> Lines { get; set; } = new List<SaleProductionLineDisplay>();
    }
}
