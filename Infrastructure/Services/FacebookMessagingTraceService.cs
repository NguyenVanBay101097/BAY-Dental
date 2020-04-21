using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FacebookMessagingTraceService : BaseService<FacebookMessagingTrace>, IFacebookMessagingTraceService
    {
        public FacebookMessagingTraceService(IAsyncRepository<FacebookMessagingTrace> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public override Task<IEnumerable<FacebookMessagingTrace>> CreateAsync(IEnumerable<FacebookMessagingTrace> entities)
        {
            _ComputeState(entities);
            return base.CreateAsync(entities);
        }

        private void _ComputeState(IEnumerable<FacebookMessagingTrace> self)
        {
            foreach(var item in self)
            {
                if (item.Exception.HasValue)
                    item.State = "exception";
                else if (item.Sent.HasValue)
                    item.State = "sent";
                else
                    item.State = "outgoing";
            }
        }
    }
}
