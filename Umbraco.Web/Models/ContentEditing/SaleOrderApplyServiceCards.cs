using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderApplyServiceCards
    {
        public Guid Id { get; set; }
        public IEnumerable<Guid> CardIds { get; set; } = new List<Guid>();
        public decimal Amount { get; set; }
    }
}
