using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class LaboOrderPrintVM
    {
        public CompanyPrintVM Company { get; set; }
        
        public DateTime DateOrder { get; set; }

        public string DoctorName { get; set; }

        public string CustomerName { get; set; }

        public string PartnerName { get; set; }

        public DateTime? DatePlanned { get; set; }

        public string Name { get; set; }

        public string PartnerRef { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerPhone { get; set; }

        public IEnumerable<LaboOrderLinePrintVM> OrderLines { get; set; } = new List<LaboOrderLinePrintVM>();

        public decimal AmountTotal { get; set; }
    }
}
