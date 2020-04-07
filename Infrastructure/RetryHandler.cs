using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class RetryHandler: DelegatingHandler
    {
        private readonly int MaxRetries;

        public RetryHandler(HttpMessageHandler innerHandler, int retryCount = 3)
            : base(innerHandler)
        {
            MaxRetries = retryCount;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (var i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }
}
