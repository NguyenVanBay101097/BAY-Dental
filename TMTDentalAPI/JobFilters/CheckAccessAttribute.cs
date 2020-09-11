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
using System.Security.Claims;
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
        public string Actions { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //nếu user id null thì unauthorize
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                //Lấy ra danh sách những permission của user
                var roleFunctionObj = (IApplicationRoleFunctionService)context.HttpContext.RequestServices.GetService(typeof(IApplicationRoleFunctionService));
                var permissions = Actions.Split(",");
                var accessResult = await roleFunctionObj.HasAccess(permissions);
                if (!accessResult.Access)
                {
                    var result = new JsonResult(new { message = accessResult.Error });
                    result.StatusCode = 403;
                    context.Result = result;
                }
            }
        }
    }
}
