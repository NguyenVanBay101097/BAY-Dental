using ApplicationCore.Entities;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrSalaryConfigSave
    {
        public Guid DefaultGlobalLeaveTypeId { get; set; }
        public Guid CompanyId { get; set; }

    }
    public class HrSalaryConfigDisplay
    {
        public Guid? Id { get;set; }
        public Guid? DefaultGlobalLeaveTypeId { get; set; }
        public Guid CompanyId { get; set; }
        public WorkEntryTypeBasic DefaultGlobalLeaveType { get; set; }
    }

    public class HrSalaryConfigDefault
    {
        public Guid CompanyId { get; set; }
    }
}
