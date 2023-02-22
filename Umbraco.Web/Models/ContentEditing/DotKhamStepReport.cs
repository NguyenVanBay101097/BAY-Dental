using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepReport
    {
        public string StepName { get; set; }
        public string ServiceName { get; set; }
        public string PartnerName { get; set; }
        public DateTime? Date { get; set; }
        public string DoctorName { get; set; }
        public string AssistantName { get; set; }
    }
}
