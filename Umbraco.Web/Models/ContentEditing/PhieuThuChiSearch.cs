using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiSearch
    {
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public Guid? CompanyId { get; set; }
        public string Type { get; set; }
    }

    public class ReportPhieuThuChi
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public string Type { get; set; }
    }
}
