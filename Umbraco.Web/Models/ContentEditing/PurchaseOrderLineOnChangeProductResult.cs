using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderLineOnChangeProductResult
    {
        public string Name { get; set; }
        public decimal PriceUnit { get; set; }
        public Guid? ProductUOMId { get; set; }
        public UoMDisplay ProductUOM { get; set; }
        public Guid? ProductUOMPOId { get; set; }
        public UoMDisplay ProductUOMPO { get; set; }
    }
}
