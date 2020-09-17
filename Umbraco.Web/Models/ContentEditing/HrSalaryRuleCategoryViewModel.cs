using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class HrSalaryRuleCategoryPaged
    {
        public HrSalaryRuleCategoryPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Filter { get; set; }
    }

    public class HrSalaryRuleCategorySave
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public Guid? ParentId { get; set; }
    }

    public class HrSalaryRuleCategoryDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public Guid? ParentId { get; set; }
        public HrSalaryRuleCategory Parent { get; set; }
    }
}
