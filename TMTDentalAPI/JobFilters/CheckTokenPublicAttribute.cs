
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Umbraco.Web.Models.ContentEditing;
using Microsoft.Extensions.Primitives;

namespace TMTDentalAPI.JobFilters
{
    public class CheckTokenPublicAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestSerivce = context.HttpContext.RequestServices;
            var irConfigParameter = requestSerivce.GetService<IIrConfigParameterService>();
            var tokenHeader = context.HttpContext.Request.Headers["public_token"];
            var public_token = irConfigParameter.GetParam("public.access_token").Result;
            if(string.IsNullOrEmpty(public_token) || string.IsNullOrEmpty(public_token) || public_token != tokenHeader)
            {
                context.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
