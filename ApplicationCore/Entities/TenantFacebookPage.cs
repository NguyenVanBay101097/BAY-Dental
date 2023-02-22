using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TenantFacebookPage : AdminBaseEntity
    {
        public Guid TenantId { get; set; }
        public AppTenant Tenant { get; set; }

        public string PageId { get; set; }
    }
}
