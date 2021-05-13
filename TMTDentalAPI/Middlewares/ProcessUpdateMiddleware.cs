﻿using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using Infrastructure.Data;
using Infrastructure.TenantData;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            //neeus url cua request ma == 'ProcessUpdate'
            if (context.Request.Path.Equals(new PathString("/ProcessUpdate")))
            {
                var tenantContext = context.GetTenantContext<AppTenant>();
                var tenant = tenantContext?.Tenant;
                if (tenant != null)
                {
                    try
                    {
                        var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
                        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                            await dbContext.Database.MigrateAsync();

                        await _mediator.Publish(new ProcessUpdateNotification(context));

                        var tenantDbContext = (TenantDbContext)context.RequestServices.GetService(typeof(TenantDbContext));
                        var tnt = tenantDbContext.Tenants.Where(x => x.Hostname == tenant.Hostname).FirstOrDefault();
                        tnt.Version = _appSettings.Version;
                        tenantDbContext.SaveChanges();

                        _cache.Remove(tenant.Hostname); //clear cache
                        tenant.Version = _appSettings.Version;

                        context.SetTenantContext(tenantContext);

                        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        context.Response.Headers.Add("Pragma", new[] { "no-cache" });
                        context.Response.Headers.Add("Cache-Control", new[] { "no-cache,no-store" });
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
