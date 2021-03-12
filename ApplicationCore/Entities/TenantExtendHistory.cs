using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TenantExtendHistory : AdminBaseEntity
    {
        public TenantExtendHistory()
        {
            StartDate = DateTime.Now;
        }

        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ActiveCompaniesNbr { get; set; }

        public Guid TenantId { get; set; }
        public AppTenant AppTenant { get; set; }
    }
}
