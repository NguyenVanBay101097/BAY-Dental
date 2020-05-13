using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderLineOnChangeProduct
    {
        public Guid? ProductId { get; set; }
        public Guid? UOMId { get; set; }
    }
}
