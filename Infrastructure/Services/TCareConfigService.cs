using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TCareConfigService : BaseService<TCareConfig>, ITCareConfigService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        public TCareConfigService(IAsyncRepository<TCareConfig> repository,
            IHttpContextAccessor httpContextAccessor,ITenant<AppTenant> tenant,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
        }

        public async Task<TCareConfig> GetFirst()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            if (res == null)
            {
                // add jobs and new record config
                var newConfig = new TCareConfig();
                var tenant = _tenant != null ? _tenant.Hostname : "localhost";
                RecurringJob.AddOrUpdate<TCareCampaignJobService>($"{tenant}-tcare-campaign-job", x => x.Run(tenant, null), $"{newConfig.JobCampaignMinute} {newConfig.JobCampaignHour} * * *", TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate<TCareMessagingJobService>($"{tenant}-tcare-messaging-job", x => x.ProcessQueue(tenant), $"*/{newConfig.JobMessagingMinute} * * * *", TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate<TCareMessageJobService>($"{tenant}-tcare-message-job", x => x.Run(tenant), $"*/{newConfig.JobMessageMinute} * * * *", TimeZoneInfo.Local);

                res = await CreateAsync(newConfig);
            }
            return res;
        }
    }
}
