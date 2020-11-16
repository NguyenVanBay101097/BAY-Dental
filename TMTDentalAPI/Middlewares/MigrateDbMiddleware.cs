using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using HtmlAgilityPack;
using Infrastructure.Data;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares
{
    public class MigrateDbMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _cache;

        public MigrateDbMiddleware(RequestDelegate next, IOptions<AppSettings> config, IMemoryCache cache)
        {
            _next = next;
            _appSettings = config?.Value;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //var key = AppConstants.GetLockRequestKey(context.Request.Host.Host, "__migrate");
            //var lockObj = LockUtils.Get(key);
            //await lockObj.WaitAsync();

            try
            {
                var tenant = context.GetTenant<AppTenant>();
                if (tenant != null && _appSettings.Version != tenant.Version)
                {
                    var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
                    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                        await dbContext.Database.MigrateAsync();

                    //update version
                    var tenantContext = (TenantDbContext)context.RequestServices.GetService(typeof(TenantDbContext));
                    var tnt = await tenantContext.Tenants.Where(x => x.Hostname == tenant.Hostname).FirstOrDefaultAsync();
                    if (tnt != null)
                    {
                        tnt.Version = _appSettings.Version;
                        tenantContext.SaveChanges();

                        _cache.Remove(tenant.Hostname); //clear cache
                    }
                }
            }
            catch
            {
            }

            await _next.Invoke(context);

            //lockObj.Release();
        }
    }
}
