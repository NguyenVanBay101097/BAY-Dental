using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares
{
    public class GetTokenFromQueryStringMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="next">the next middleware in chain</param>
        public GetTokenFromQueryStringMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// sets the header
        /// </summary>
        /// <param name="context">Current HTTP context</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
            {
                if (context.Request.QueryString.HasValue)
                {
                    var token = context.Request.QueryString.Value.Split('&')
                        .SingleOrDefault(x => x.Contains("token"))?.Split('=')[1];
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                    }
                }
            }
            await _next.Invoke(context);
        }
    }
}
