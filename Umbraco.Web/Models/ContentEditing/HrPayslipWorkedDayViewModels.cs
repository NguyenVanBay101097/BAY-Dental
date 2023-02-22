using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayslipWorkedDayPaged
    {

        public HrPayslipWorkedDayPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public Guid? PayslipId { get; set; }
    }

    public class HrPayslipWorkedDaySave
    {
        public Guid Id { get; set; }

        public decimal? NumberOfDays { get; set; }

        public decimal? NumberOfHours { get; set; }

        public Guid WorkEntryTypeId { get; set; }

        public decimal? Amount { get; set; }

        public string Name { get; set; }
    }

    public class HrPayslipWorkedDayDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal? NumberOfDays { get; set; }

        public decimal? NumberOfHours { get; set; }

        public decimal? Amount { get; set; }

        public Guid WorkEntryTypeId { get; set; }
    }

    public class HrPayslipWorkedDayBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal? NumberOfDays { get; set; }

        public decimal? NumberOfHours { get; set; }

        public decimal? Amount { get; set; }
        public Guid? WorkEntryTypeId { get; set; }
    }
}
