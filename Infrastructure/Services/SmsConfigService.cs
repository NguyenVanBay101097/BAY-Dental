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
            if (model.IsAppointmentAutomation)
            {
                var jobId = $"{hostName}_AppointmentAutomaticReminder_{model.Id}";
                RecurringJob.AddOrUpdate<ISmsJobService>(jobId, x => x.RunJob(hostName, model.Id), $"*/5 * * * *", TimeZoneInfo.Local);
                model.JobId = jobId;
            }

            else if (model.IsBirthdayAutomation)
            {
                var jobId = $"{hostName}_BirthdayAutomaticReminder_{model.Id}";
                RecurringJob.AddOrUpdate<ISmsJobService>(jobId, x => x.RunJob(hostName, model.Id), $"* 8 * * *", TimeZoneInfo.Local);
                model.JobId = jobId;
            }

            else if (!string.IsNullOrEmpty(model.JobId) && (!model.IsBirthdayAutomation || !model.IsAppointmentAutomation))
            {
                ActionStopJob(model.JobId);
                model.JobId = "";
            }

            Update(model);
        }

        public void ActionStopJob(string jobId)
        {
            BackgroundJob.Delete(jobId);
        }
    }
}
