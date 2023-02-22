using Hangfire.Client;
using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.Services;

namespace TMTDentalAPI.JobFilters
{
    public class ClientTenantFilter : JobFilterAttribute, IClientFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnCreating(CreatingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));
            filterContext.SetJobParameter("tenant_id", "1234");
        }
    }
}
