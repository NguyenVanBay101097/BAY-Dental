using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepPaged
    {
        public DotKhamStepPaged()
        {
            Limit = 20;
            Offset = 0;
        }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public string UserId { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? DoctorId { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public int Offset { get; set; }
    }
}
