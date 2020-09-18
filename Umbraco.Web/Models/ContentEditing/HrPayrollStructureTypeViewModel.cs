using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayrollStructureTypePaged
    {

        public HrPayrollStructureTypePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
    }

    public class HrPayrollStructureTypeSave
    {
        public Guid? DefaultResourceCalendarId { get; set; }
        public string DefaultSchedulePay { get; set; }
        //public Guid? DefaultStructId { get; set; }
        //public Guid? DefaultWorkEntryTypeId { get; set; }
        public string Name { get; set; }
        public string WageType { get; set; }
    }
    public class HrPayrollStructureTypeDisplay
    {
        public Guid Id { get; set; }
        public Guid? DefaultResourceCalendarId { get; set; }
        public ResourceCalendarDisplay ResourceCalendar { get; set; }
        public string DefaultSchedulePay { get; set; }
        public Guid? DefaultStructId { get; set; }
        public HrPayrollStructureDisplay DefaultStruct { get; set; }
        public Guid? DefaultWorkEntryTypeId { get; set; }
        public string Name { get; set; }
        public string WageType { get; set; }
    }

    public class HrPayrollStructureTypeSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string WageType { get; set; }
    }
}
