using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderDefaultGet
    {
        public bool? IsQuotation { get; set; }

        public Guid? PartnerId { get; set; }
    }
}
