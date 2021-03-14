using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using Infrastructure.TenantData;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.Middlewares.ProcessUpdateHandlers;

namespace TMTDentalAPI.Middlewares
{
    public class ProcessUpdateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _cache;
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessUpdateMiddleware> _logger;

        public ProcessUpdateMiddleware(RequestDelegate next, IOptions<AppSettings> config, IMemoryCache cache, IMediator mediator, ILogger<ProcessUpdateMiddleware> logger)
        {
            _next = next;
            _appSettings = config?.Value;
            _cache = cache;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenantContext = context.GetTenantContext<AppTenant>();
            if (tenantContext == null || tenantContext.Tenant == null)
                await _next.Invoke(context);
            else
            {
                var key = AppConstants.GetLockRequestKey(context.Request.Host.Host, "__process_update");
                var lockObj = LockUtils.Get(key);
                try
                {
                    await lockObj.WaitAsync();
                    await _mediator.Publish(new ProcessUpdateNotification());

                    var tenant = tenantContext.Tenant;
                    var tenantDbContext = (TenantDbContext)context.RequestServices.GetService(typeof(TenantDbContext));
                    var tnt = tenantDbContext.Tenants.Where(x => x.Hostname == tenant.Hostname).FirstOrDefault();
                    tnt.Version = _appSettings.Version;
                    tenantDbContext.SaveChanges();

                    _cache.Remove(tenant.Hostname); //clear cache
                    tenant.Version = _appSettings.Version;

                    context.SetTenantContext(tenantContext);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
                finally
                {
                    lockObj.Release();
                }

                await _next.Invoke(context);
            }
        }
    }
}
