using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class MarketingTraceService : BaseService<MarketingTrace>, IMarketingTraceService
    {
        public MarketingTraceService(IAsyncRepository<MarketingTrace> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }
    }
}
