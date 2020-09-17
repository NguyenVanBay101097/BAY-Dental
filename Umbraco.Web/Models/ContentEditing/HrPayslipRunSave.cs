using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayslipRunSave
    {    

        public string Name { get; set; }
  
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public Guid CompanyId { get; set; }
        public string State { get; set; }
    }
}
