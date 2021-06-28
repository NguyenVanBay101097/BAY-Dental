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

        public CheckTenantMiddleware(RequestDelegate next, IOptions<AppSettings> config
            )
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

                subDomain = subDomain.Trim().ToLower();
                var _tenantDbContext = (TenantDbContext)context.RequestServices.GetService(typeof(TenantDbContext));
                var existTenant = _tenantDbContext.Tenants.Any(x => x.Hostname == subDomain);
                if (!existTenant)
                {
                    //var result = JsonConvert.SerializeObject(new { error = "Chưa đăng kí tài khoản", message = "Chưa đăng kí tài khoản", url = "/auth/notavaliable" });
                    //context.Response.ContentType = "application/json";
                    //await context.Response.WriteAsync(content);

                    context.Response.StatusCode = 503;

                    var content = File.ReadAllText(@"Views\Auth\Notavaliable.cshtml");
                    await context.Response.WriteAsync(content);
                }
            }

        }

    }
}
