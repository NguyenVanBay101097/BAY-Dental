using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class VFundBookSearch
    {
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public Guid? JournalId { get; set; }
        public string Type { get; set; }
        public Guid? CompanyId { get; set; }
        public string Search { get; set; }
    }
}
