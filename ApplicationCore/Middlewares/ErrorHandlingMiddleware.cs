using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationCore.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var errorConverter = (IExceptionToErrorConverter)context.RequestServices.GetService(typeof(IExceptionToErrorConverter));

            var error = errorConverter.Convert(ex);
          
            var result = JsonConvert.SerializeObject(error, new JsonSerializerSettings { 
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }

        private static RemoteServiceErrorInfo SerializeException(Exception ex)
        {
            var errorInfo = new RemoteServiceErrorInfo
            {
                Message = ex.Message,
                Name = ex.GetType().Name
            };

            return errorInfo;
        }
    }
}
