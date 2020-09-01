using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLinePaymentRelSave
    {

        public Guid SaleOrderLineId { get; set; }
        public decimal? AmountPrepaid { get; set; }

    }
}
