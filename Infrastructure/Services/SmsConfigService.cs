using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Hangfire;
using Infrastructure.HangfireJobService;
using Microsoft.AspNetCore.Http;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SmsConfigService : BaseService<SmsConfig>, ISmsConfigService
    {
        private readonly AppTenant _tenant;
        public SmsConfigService(ITenant<AppTenant> tenant, IAsyncRepository<SmsConfig> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
        }
        public override async Task<SmsConfig> CreateAsync(SmsConfig entity)
        {
            entity = await base.CreateAsync(entity);
            ActionRunJob(entity);
            return entity;
        }

        public override Task UpdateAsync(SmsConfig entity)
        {
            ActionRunJob(entity);
            return base.UpdateAsync(entity);
        }

        public void ActionRunJob(SmsConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobIdApp = $"{hostName}_Sms_AppointmentAutomaticReminder";
            var jobIdBir = $"{hostName}_Sms_BirthdayAutomaticReminder";

            if (model.IsAppointmentAutomation)
            {
                RecurringJob.AddOrUpdate<ISmsJobService>(jobIdApp, x => x.RunJob(hostName, model.Id), $"*/5 * * * *", TimeZoneInfo.Local);
            }
            else
            {
                ActionStopJob(jobIdApp);
            }

            if (model.IsBirthdayAutomation)
            {
                RecurringJob.AddOrUpdate<ISmsJobService>(jobIdBir, x => x.RunJob(hostName, model.Id), $"0 8 * * *", TimeZoneInfo.Local);
            }
            else
            {
                ActionStopJob(jobIdBir);
            }
        }

        public void ActionStopJob(string jobId)
        {
            BackgroundJob.Delete(jobId);
        }
    }
}
