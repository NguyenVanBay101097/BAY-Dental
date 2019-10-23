using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TenantUpdateDateExpiredViewModel
    {
        public Guid Id { get; set; }
        public DateTime? DateExpired { get; set; }
    }
}
