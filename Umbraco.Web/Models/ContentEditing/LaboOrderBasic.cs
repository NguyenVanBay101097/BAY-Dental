using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderBasic
    {
        public Guid Id { get; set; }
        public string PartnerName { get; set; }
        public string CustomerName { get; set; }
        public string Name { get; set; }
        public DateTime DateOrder { get; set; }
        public string State { get; set; }
        public decimal AmountTotal { get; set; }
        public DateTime? DatePlanned { get; set; }
        public string SaleOrderName { get; set; }
        public Guid? SaleOrderId { get; set; }
    }
}
