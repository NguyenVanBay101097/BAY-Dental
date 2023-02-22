using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineProductRequestedPaged
    {
        public SaleOrderLineProductRequestedPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public IEnumerable<Guid> SaleOrderLineIds { get; set; }
    }

    public class SaleOrderLineProductRequestedBasic
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public Guid ProductId { get; set; }
        public decimal RequestedQuantity { get; set; }
    }

    public class SaleOrderLineProductRequestedSave
    {
        public Guid SaleOrderLineId { get; set; }
        public Guid ProductId { get; set; }
        public decimal RequestedQuantity { get; set; }
    }

    public class SaleOrderLineProductRequestedDisplay
    {
        public Guid Id { get; set; }
        public Guid SaleOrderLineId { get; set; }
        public Guid ProductId { get; set; }
        public decimal RequestedQuantity { get; set; }
    }
}
