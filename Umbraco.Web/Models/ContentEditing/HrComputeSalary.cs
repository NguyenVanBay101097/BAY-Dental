using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class HrComputeSalary
    {
        public Guid? PaySlipId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
