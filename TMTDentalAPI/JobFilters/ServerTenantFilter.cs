using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.JobFilters
{
    public class ServerTenantFilter : IServerFilter
    {
        public void OnPerformed(PerformedContext filterContext)
        {
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException(nameof(filterContext));

            var tenant_id = filterContext.GetJobParameter<string>("tenant_id");
        }
    }
}
