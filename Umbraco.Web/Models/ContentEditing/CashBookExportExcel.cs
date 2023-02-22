using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CashBookExportExcel
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Ref { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string PartnerName { get; set; }
    }
}
