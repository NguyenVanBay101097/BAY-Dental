using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            //nen check version de han che viec migrate hay ko?
            var tenant = context.GetTenant<AppTenant>();
            if (tenant != null)
            {
                //Microsoft.Extensions.Primitives.StringValues skipCheck = "";
                //if (!context.Request.Query.TryGetValue("skipCheckExpired", out skipCheck))
                //{
                //    var now = DateTime.Now;
                //    if (tenant.DateExpired.HasValue && tenant.DateExpired.Value <= now)
                //    {
                //        await HandleExpiredAsync(context);
                //    }
                //}

                var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync();
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        private static Task HandleExpiredAsync(HttpContext context)
        {
            var code = HttpStatusCode.Gone; // 410

            //if (ex is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (ex is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (ex is MyException) code = HttpStatusCode.BadRequest;
            var result = JsonConvert.SerializeObject(new { error = "Tai khoan cua ban da het han." });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
