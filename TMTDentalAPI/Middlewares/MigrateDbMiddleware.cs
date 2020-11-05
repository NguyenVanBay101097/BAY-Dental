using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using HtmlAgilityPack;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

        public MigrateDbMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = AppConstants.GetLockRequestKey(context.Request.Host.Host, "__migrate");
            var lockObj = LockUtils.Get(key);
            await lockObj.WaitAsync();

            try
            {
                var tenant = context.GetTenant<AppTenant>();
                if (tenant != null)
                {
                    var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
                    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                        await dbContext.Database.MigrateAsync();
                }

                await _next(context);
            }
            finally
            {
                lockObj.Release();
            }
        }
    }
}
