using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrSalaryRulePaged
    {

        public HrSalaryRulePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Filter { get; set; }
        public Guid? StructId { get; set; }
    }
    public class HrSalaryRuleSave
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public Guid? CategoryId { get; set; }

        public int? Sequence { get; set; }

        public bool Active { get; set; }

        public string AmountSelect { get; set; }
        public decimal? AmountFix { get; set; }
        public decimal? AmountPercentage { get; set; }

        public bool AppearsOnPayslip { get; set; }

        public string Note { get; set; }
        public string AmountCodeCompute { get; set; }
        public string AmountPercentageBase { get; set; }

        //public Guid? StructId { get; set; }
    }
    public class HrSalaryRuleDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public int? Sequence { get; set; }

        public bool Active { get; set; }

        public Guid? CompanyId { get; set; }

        public string AmountSelect { get; set; }
        public decimal? AmountFix { get; set; }
        public decimal? AmountPercentage { get; set; }

        public bool AppearsOnPayslip { get; set; }

        public string Note { get; set; }
        public string AmountCodeCompute { get; set; }

        public string AmountPercentageBase { get; set; }

        public Guid StructId { get; set; }
    }
}
