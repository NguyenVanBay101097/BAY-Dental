using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderPrintVM
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }

        public string PartnerRef { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerPhone { get; set; }

        public string Name { get; set; }
        public DateTime DateOrder { get; set; }

        public IEnumerable<LaboOrderLinePrintVM> OrderLines { get; set; } = new List<LaboOrderLinePrintVM>();

        public decimal AmountTotal { get; set; }
    }
}
