using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TMTDentalAPI.JobFilters
{
    [AttributeUsage(AttributeTargets.Class |
                            AttributeTargets.Method
                       , AllowMultiple = true
                       , Inherited = true)]
    public class CheckAccessAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _permission;
        public CheckAccessAttribute(params string[] permission)
        {
            _permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var funcObj = (IApplicationRoleFunctionService)context.HttpContext.RequestServices.GetService(typeof(IApplicationRoleFunctionService));
            var access = await funcObj.HasAccess(_permission.ToList());
            if (!access)
            {
                var result = new JsonResult(new { message = "Bạn không có quyền cho thao tác này!" });
                result.StatusCode = 403;
                context.Result = result;
            }
            return;
        }
    }

}
