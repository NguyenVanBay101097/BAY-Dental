using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CashBookReport
    {
        public CashBookReport()
        {
            Begin = 0;
            TotalThu = 0;
            TotalChi = 0;
            TotalAmount = 0;
        }
        public decimal Begin { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalChi { get; set; }
        public decimal TotalThu { get; set; }
    }
}
