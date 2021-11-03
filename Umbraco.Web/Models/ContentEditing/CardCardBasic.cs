using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CardCardBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PartnerName { get; set; }
        public string PartnerPhone { get; set; }
        public string TypeName { get; set; }
        public Guid TypeId { get; set; }
        public decimal? TotalPoint { get; set; }
        public decimal? PointInPeriod { get; set; }
        public string Barcode { get; set; }
        public string State { get; set; }
    }
}
