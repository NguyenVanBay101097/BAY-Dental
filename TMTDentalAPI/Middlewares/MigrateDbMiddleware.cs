using ApplicationCore.Entities;
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
            //nen check version de han che viec migrate hay ko?
            //var tenant = context.GetTenant<AppTenant>();
            //if (tenant != null)
            //{
            //    //Microsoft.Extensions.Primitives.StringValues skipCheck = "";
            //    //if (!context.Request.Query.TryGetValue("skipCheckExpired", out skipCheck))
            //    //{
            //    //    var now = DateTime.Now;
            //    //    if (tenant.DateExpired.HasValue && tenant.DateExpired.Value <= now)
            //    //    {
            //    //        //await HandleExpiredAsync(context);
            //    //    }
            //    //}

            //    var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
            //    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            //    if (pendingMigrations.Any())
            //        await dbContext.Database.MigrateAsync();
            //}

            var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync();

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        //private static Task HandleExpiredAsync(HttpContext context)
        //{
        //    return context.Response.WriteHtmlAsync("<html>" +
        //        "<head>" +
        //            "<link href=\"/css/bootstrap.min.css\" rel=\"stylesheet\">" +
        //        "</head>" +
        //        "<body>" +
        //            "<div class=\"jumbotron\">" +
        //                "<h1 class=\"display-4\">Hết hạn!</h1>" +
        //                "<p class=\"lead\">Ứng dụng đã hết hạn, vui lòng liên hệ hotline <strong>0908075455</strong> để được hỗ trợ.</p>" +
        //            "</div>" +
        //        "</body>" +
        //        "</html>");
        //}
    }
}
