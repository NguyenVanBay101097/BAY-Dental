using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderBasic
    {
        public Guid Id { get; set; }
        public string PartnerName { get; set; }
        public string Name { get; set; }
        public DateTime DateOrder { get; set; }
        public string State { get; set; }
        public decimal? AmountTotal { get; set; }
        public string Type { get; set; }
        public decimal AmountResidual { get; set; }
    }
}
