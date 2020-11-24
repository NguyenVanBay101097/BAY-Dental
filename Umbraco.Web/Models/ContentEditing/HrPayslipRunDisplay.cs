using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayslipRunDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public Guid CompanyId { get; set; }
        public CompanyBasic Company { get; set; }

        public string State { get; set; }
        public DateTime? Date { get; set; }
        public IEnumerable<HrPayslipDisplay> Slips { get; set; } = new List<HrPayslipDisplay>();
        public ApplicationUserSimple User { get; set; }
        public bool IsExistSalaryPayment { get; set; }

    }
}
