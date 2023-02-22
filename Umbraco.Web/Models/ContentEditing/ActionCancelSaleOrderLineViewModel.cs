using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class ActionCancelSaleOrderLineViewModel
    {
        public Guid SaleOrderId { get; set; }
        public Guid SaleOrderLineId { get; set; }
    }
}
