﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record.Chart;
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

        public async Task<TCareConfig> GetConfig()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            if (res == null)
            {
                // add jobs and new record config
                var newConfig = new TCareConfig();
                var tenant = _tenant != null ? _tenant.Hostname : "localhost";
                try
                {
                    RecurringJob.AddOrUpdate<TCareCampaignJobService>($"{tenant}-tcare-campaign-job", x => x.Run(tenant, null), $"{newConfig.JobCampaignMinute} {newConfig.JobCampaignHour} * * *", TimeZoneInfo.Local);

                    var messagingMinute = newConfig.JobMessagingMinute ?? 60;
                    var messagingTimeSpan = TimeSpan.FromMinutes(messagingMinute);
                    RecurringJob.AddOrUpdate<TCareMessagingJobService>($"{tenant}-tcare-messaging-job", x => x.ProcessQueue(tenant), $"{(messagingTimeSpan.Minutes > 0 ? "*/" + messagingTimeSpan.Minutes : "*")} {(messagingTimeSpan.Hours > 0 ? "*/" + messagingTimeSpan.Hours : "*")} * * *", TimeZoneInfo.Local);

                    var messageMinute = newConfig.JobMessageMinute ?? 60;
                    var messageTimeSpan = TimeSpan.FromMinutes(messageMinute);
                    RecurringJob.AddOrUpdate<TCareMessageJobService>($"{tenant}-tcare-message-job", x => x.Run(tenant), $"{(messageTimeSpan.Minutes > 0 ? "*/" + messageTimeSpan.Minutes : "*")} {(messageTimeSpan.Hours > 0 ? "*/" + messageTimeSpan.Hours : "*")} * * *", TimeZoneInfo.Local);
                }
                catch(Exception e)
                {
                    throw e;
                }
              
                res = await CreateAsync(newConfig);
            }
            return res;
        }
    }
}
