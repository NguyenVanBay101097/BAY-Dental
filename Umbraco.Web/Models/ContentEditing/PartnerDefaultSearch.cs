using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerDefaultSearch
    {
        public Guid? PartnerId { get; set; }
        public IEnumerable<Guid> InvoiceIds { get; set; } = new List<Guid>();
        public Guid? CompanyId { get; set; }
    }
}
