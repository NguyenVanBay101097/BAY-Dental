using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class TCareMessagingTraceService : BaseService<TCareMessagingTrace>, ITCareMessagingTraceService
    {
        public TCareMessagingTraceService(IAsyncRepository<TCareMessagingTrace> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {

        }
    }
}
