using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayrollStructureBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class HrPayrollStructurePaged
    {

        public HrPayrollStructurePaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Filter { get; set; }
        public Guid? StructureTypeId { get; set; }
    }

    public class HrPayrollStructureSave
    {
        public string Name { get; set; }

        public bool RegularPay { get; set; }

        public Guid TypeId { get; set; }

        public IEnumerable<HrSalaryRuleSave> Rules { get; set; } = new List<HrSalaryRuleSave>();
    }

    public class HrPayrollStructureDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool RegularPay { get; set; }
        public Guid TypeId { get; set; }
        public HrPayrollStructureTypeDisplay Type { get; set; }
        public IEnumerable<HrSalaryRuleDisplay> Rules { get; set; } = new List<HrSalaryRuleDisplay>();
    }

    public class HrPayrollStructureBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class HrPayrollStructureDefaultGetResult
    {
        public Guid CompanyId { get; set; }
    }
}
