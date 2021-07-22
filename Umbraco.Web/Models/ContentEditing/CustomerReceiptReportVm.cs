using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CustomerReceiptReportFilter
    {
        public CustomerReceiptReportFilter()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public double? TimeFrom { get; set; }
        public double? TimeTo { get; set; }

        public string Search { get; set; }
        public Guid? CompanyId { get; set; }

        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Khách hàng tái khám
        /// </summary>
        public bool? IsRepeatCustomer { get; set; }

        /// <summary>
        /// không điều trị
        /// </summary>
        public bool? IsNoTreatment { get; set; }

        public string state { get; set; }

    }

    
}
