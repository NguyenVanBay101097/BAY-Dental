using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class MarketingMessageService : BaseService<MarketingMessage>, IMarketingMessageService
    {
        public MarketingMessageService(IAsyncRepository<MarketingMessage> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
        }
    }
}
