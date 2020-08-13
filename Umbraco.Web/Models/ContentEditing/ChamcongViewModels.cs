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

        public employeePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public string Filter { get; set; }
        public int Offset { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Status { get; set; }
        public Guid? EmployeeId { get; set; }

    }

    public class ChamCongSave
    {
        public Guid EmployeeId { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

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
            get
            {
                if (this.TimeIn.HasValue && this.TimeOut.HasValue)
                {
                    return "done";
                }
                else if (!this.TimeOut.HasValue && !this.TimeIn.HasValue)
                {
                    if (this.Status == "NP")
                    {
                        return "NP";
                    }
                    return "initial";
                }
                return "process";
            }
            set { }
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

        public Guid CompanyId { get; set; }
        public string Status { get; set; }
        public EmployeeBasic employee { get; set; }
        public Guid? WorkEntryTypeId { get; set; }
        public WorkEntryTypeDisplay WorkEntryType { get; set; }
    }
}
