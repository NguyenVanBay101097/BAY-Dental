using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.ActionFilters
{
    public class CheckTenantExpiredActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var tenant = context.HttpContext.GetTenant<AppTenant>();
            var now = DateTime.Now;
            if (tenant != null && tenant.DateExpired.HasValue && tenant.DateExpired.Value <= now)
            {
                var result = new JsonResult(new { error = "Hết hạn sử dụng phần mềm", message = "Hết hạn sử dụng phần mềm", url = "/auth/expired" });
                result.ContentType = "application/json";
                result.StatusCode = 410;
                context.Result = result;
                return;
            }
        }
    }
}
