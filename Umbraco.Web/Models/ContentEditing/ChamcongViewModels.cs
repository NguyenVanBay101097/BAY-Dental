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

        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        //public Guid WorkEntryTypeId { get; set; }

        public string Type { get; set; }

        public DateTime? Date { get; set; }
        /// <summary>
        /// tăng ca
        /// </summary>
        public bool OverTime { get; set; }

        /// <summary>
        /// số giờ tăng ca
        /// </summary>
        public decimal? OverTimeHour { get; set; }

        public Guid CompanyId { get; set; }
    }

    public class ChamCongDisplay
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public DateTime Date { get; set; }

        public Guid? WorkEntryTypeId { get; set; }
        public WorkEntryTypeBasic WorkEntryType { get; set; }

        public Guid CompanyId { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// tăng ca
        /// </summary>
        public bool OverTime { get; set; }

        /// <summary>
        /// số giờ tăng ca
        /// </summary>
        public decimal? OverTimeHour { get; set; }
    }

    public class ChamCongExportExcell
    {
        public Guid? EmployeeId { get; set; }
        public EmployeeDisplay Employee { get; set; }
        public IEnumerable<DateTime> Dates { get; set; } = new List<DateTime>();
    }

    public class ChamCongDefaultGetPost
    {
        public Guid? EmployeeId { get; set; }
    }

    public class ChamCongDefaultGetResult
    {
        public Guid? CompanyId { get; set; }

        public Guid? WorkEntryTypeId { get; set; }
        public WorkEntryTypeBasic WorkEntryType { get; set; }

        public Guid? EmployeeId { get; set; }
        public EmployeeBasic Employee { get; set; }
    }

    public class TaoChamCongNguyenNgayViewModel
    {
        public DateTime Date { get; set; }
        public Guid WorkEntryTypeId { get; set; }
    }

    public class TaoChamCongNguyenThangViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
