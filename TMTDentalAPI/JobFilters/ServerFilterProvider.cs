using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.JobFilters
{
    public class ServerFilterProvider : IJobFilterProvider
    {
        public IEnumerable<JobFilter> GetFilters(Job job)
        {
            return new JobFilter[]
                       {
                       new JobFilter(new ServerTenantFilter (), JobFilterScope.Global,  null),
                       };
        }
    }
}
