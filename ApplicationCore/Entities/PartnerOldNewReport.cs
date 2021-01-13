using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerOldNewReport
    {
        public DateTime Date { get; set; }
        public string PartnerName { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public string Type { get; set; }
        public string OrderName { get; set; }
        public Guid OrderId { get; set; }
        public SaleOrder Order { get; set; }
        public int CountLine { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
