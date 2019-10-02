using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var dbContext = (CatalogDbContext)context.RequestServices.GetService(typeof(CatalogDbContext));
                await dbContext.Database.MigrateAsync();
            }
           
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
