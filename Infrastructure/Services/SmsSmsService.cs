using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SmsSmsService : BaseService<SmsSms>, ISmsSmsService
    {
        public SmsSmsService(IAsyncRepository<SmsSms> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }
    }
}
