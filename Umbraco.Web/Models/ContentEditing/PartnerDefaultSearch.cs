using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerDefaultSearch
    {
        public Guid? PartnerId { get; set; }
        public IEnumerable<Guid> InvoicesIds { get; set; } = new List<Guid>();
        public string ResultSelection { get; set; }
    }
}
