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
        public Guid CompanyId { get; set; }
        public string SchedulePay { get; set; }
        public IEnumerable<HrSalaryRuleSave> Rules { get; set; }
    }
    public class HrPayrollStructureDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public string SchedulePay { get; set; }
        public IEnumerable<HrSalaryRuleDisplay> Rules { get; set; }
        public int TotalRules { get; set; }
    }
}
