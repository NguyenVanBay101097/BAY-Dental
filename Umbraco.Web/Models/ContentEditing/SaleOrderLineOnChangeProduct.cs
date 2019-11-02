using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineOnChangeProduct
    {
        public Guid? ProductId { get; set; }

        public Guid? PartnerId { get; set; }
    }
}
