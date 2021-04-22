using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SmsComposerService : BaseService<SmsComposer>, ISmsComposerService
    {
        public SmsComposerService(IAsyncRepository<SmsComposer> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }
    }
}
