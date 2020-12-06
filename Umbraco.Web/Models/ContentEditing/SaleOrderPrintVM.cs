﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPrintVM
    {
        public Guid CompanyId { get; set; }
        public CompanyPrintVM Company { get; set; }

        public string PartnerRef { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerPhone { get; set; }

        public string Name { get; set; }
        public DateTime DateOrder { get; set; }

        public IEnumerable<SaleOrderLinePrintVM> OrderLines { get; set; } = new List<SaleOrderLinePrintVM>();

        public decimal AmountTotal { get; set; }
    }
}
