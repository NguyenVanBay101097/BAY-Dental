using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrSalaryRuleDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public Guid CategoryId { get; set; }

        public int? Sequence { get; set; }

        public bool Active { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// The computation method for the rule amount.
        /// </summary>
        public string AmountSelect { get; set; }

        /// <summary>
        /// Fixed Amount
        /// </summary>
        public decimal? AmountFix { get; set; }

        /// <summary>
        /// Percentage (%)
        /// </summary>
        public decimal? AmountPercentage { get; set; }

        public bool AppearsOnPayslip { get; set; }

        public string Note { get; set; }

        public Guid StructId { get; set; }
    }
}
