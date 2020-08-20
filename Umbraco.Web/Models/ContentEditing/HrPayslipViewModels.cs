using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayslipPaged
    {

        public HrPayslipPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }

    public class HrPayslipSave
    {
        public Guid? StructId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string State { get; set; }
        //public IEnumerable<HrPayslipLineSave> Lines { get; set; }
        //public Guid CompanyId { get; set; } get current
    }
    public class HrPayslipDisplay
    {
        public Guid id { get; set; }
        public Guid? StructId { get; set; }
        public HrPayrollStructureDisplay Struct { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public Guid EmployeeId { get; set; }
        public EmployeeBasic Employee { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string State { get; set; }
        public Guid CompanyId { get; set; }
        public IEnumerable<HrPayslipLineDisplay> Lines { get; set; }
        public IEnumerable<HrPayslipWorkedDays> WorkedDaysLines { get; set; }
    }
}
