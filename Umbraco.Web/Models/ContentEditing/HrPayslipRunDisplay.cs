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

        public string State { get; set; }

        public ICollection<HrPayslipDisplay> Slips { get; set; } = new List<HrPayslipDisplay>();
    }
}
