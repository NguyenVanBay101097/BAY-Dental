using ApplicationCore.Entities;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class employeePaged
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public string Filter { get; set; }
        public Guid? EmployeeId { get; set; }

    }

    public class ChamCongSave
    {
        public Guid EmployeeId { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime? Date { get; set; }

        public decimal? HourWorked
        {
            get
            {
                if (this.TimeIn.HasValue && this.TimeOut.HasValue)
                    return Math.Round((decimal)(this.TimeOut.Value - this.TimeIn.Value).TotalHours, 2);
                else return null;
            }
            set
            {

            }
        }

        public string Status
        {
            get; set;
        }
        public Guid WorkEntryTypeId { get; set; }

    }

    public class ChamCongDisplay
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public decimal? HourWorked { get; set; }
        public string Status { get; set; }
        public Guid? WorkEntryTypeId { get; set; }
        public WorkEntryTypeDisplay WorkEntryType { get; set; }
    }

    public class ChamCongExportExcell
    {
        public Guid? EmployeeId { get; set; }
        public EmployeeDisplay Employee { get; set; }
        public IEnumerable<DateTime> Dates { get; set; } = new List<DateTime>();
    }
}
