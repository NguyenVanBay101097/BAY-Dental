using ApplicationCore.Entities;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class CachingAppTenantResolver : MemoryCacheTenantResolver<AppTenant>
    {
        private readonly TenantDbContext _tenantDbContext;

        public CachingAppTenantResolver(IMemoryCache cache, ILoggerFactory loggerFactory, TenantDbContext tenantDbContext)
           : base(cache, loggerFactory)
        {
            _tenantDbContext = tenantDbContext;
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            var host = context.Request.Host.Host;
            var subDomain = string.Empty;
            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            subDomain = subDomain.Trim().ToLower();
            return subDomain;
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<AppTenant> context)
        {
            return new string[] { context.Tenant.Hostname };
        }

        protected override Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;
            var subDomain = string.Empty;

            var host = context.Request.Host.Host;
            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            subDomain = subDomain.Trim().ToLower();

            var tenant = _tenantDbContext.Tenants.FirstOrDefault(x => x.Hostname == subDomain);

            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }
    }
}
