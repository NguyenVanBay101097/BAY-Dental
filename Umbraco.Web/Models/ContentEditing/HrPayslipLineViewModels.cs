using ApplicationCore.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayslipLinePaged
    {

        public HrPayslipLinePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public Guid? payslipId { get; set; }
    }

    public class HrPayslipLineSave
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Total { get; set; }
        public Guid SlipId { get; set; }
        public Guid SalaryRuleId { get; set; }
        public decimal? Rate { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Sequence { get; set; }
    }
    public class HrPayslipLineDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Total { get; set; }
        public Guid SlipId { get; set; }
        public Guid SalaryRuleId { get; set; }
        public decimal? Rate { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Sequence { get; set; }
    }
}
