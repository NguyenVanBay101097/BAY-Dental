using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountRegisterPaymentDefaultGet
    {
        public IEnumerable<Guid> InvoiceIds { get; set; }
    }
}
