using ApplicationCore.Entities;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares
{
    public class CheckTenantMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckTenantMiddleware(RequestDelegate next, IOptions<AppSettings> config)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenantContext = context.GetTenantContext<AppTenant>();
            var tenant = tenantContext == null ? null : tenantContext.Tenant;
            if (tenant != null)
            {
                await _next.Invoke(context);
            }
            else
            {
                var subDomain = string.Empty;
                var host = context.Request.Host.Host;
                if (!string.IsNullOrWhiteSpace(host))
                {
                    subDomain = host.Split('.')[0];
                }

                if (subDomain == "localhost")
                {
                    await _next.Invoke(context);
                }
                else
                {
                    var content = File.ReadAllText(@"Views\Auth\Notavaliable.cshtml");
                    await context.Response.WriteAsync(content);
                }
            }
        }
    }
}
