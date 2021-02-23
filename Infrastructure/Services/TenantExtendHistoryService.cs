using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class TenantExtendHistoryService : AdminBaseService<TenantExtendHistory>, ITenantExtendHistoryService
    {
        public TenantExtendHistoryService(IAsyncRepository<TenantExtendHistory> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }

    }
}
