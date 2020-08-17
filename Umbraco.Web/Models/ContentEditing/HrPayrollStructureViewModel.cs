using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayrollStructurePaged
    {

        public HrPayrollStructurePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Filter { get; set; }
    }

    public class HrPayrollStructureSave
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public string SchedulePay { get; set; }
        public string Note { get; set; }
        public bool RegularPay { get; set; }
        public Guid TypeId { get; set; }
        public bool UseWorkedDayLines { get; set; }
        public IEnumerable<HrSalaryRuleSave> Rules { get; set; }

    }
    public class HrPayrollStructureDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string SchedulePay { get; set; }
        public string Note { get; set; }
        public bool RegularPay { get; set; }
        public Guid TypeId { get; set; }
        public bool UseWorkedDayLines { get; set; }
        public List<HrSalaryRuleDisplay> Rules { get; set; }
    }
}
