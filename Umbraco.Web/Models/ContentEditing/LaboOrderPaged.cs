using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderPaged
    {
        public LaboOrderPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? PartnerId { get; set; }
        public DateTime? DateOrderFrom { get; set; }
        public DateTime? DateOrderTo { get; set; }
        public DateTime? DatePlannedFrom { get; set; }
        public DateTime? DatePlannedTo { get; set; }
        public string State { get; set; }
        public Guid? SaleOrderId { get; set; }
    }
}
