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
            if (!entity.SmsCampaignId.HasValue && entity.Type == "birthday")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignBirthday();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "appointment")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignAppointmentReminder();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "thanks-customer")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "care-after-order")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCareAfterOrder();
                entity.SmsCampaignId = campaign.Id;
            }
            entity = await base.CreateAsync(entity);
            ActionRunJob(entity);
            return entity;
        }

        public override async Task UpdateAsync(SmsConfig entity)
        {
            ActionRunJob(entity);
            if (!entity.SmsCampaignId.HasValue && entity.Type == "birthday")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignBirthday();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "appointment")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignAppointmentReminder();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "thanks-customer")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "care-after-order")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCareAfterOrder();
                entity.SmsCampaignId = campaign.Id;
                await base.UpdateAsync(entity);
            }
        }

        public void ActionRunJob(SmsConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobIdApp = $"{hostName}_Sms_AppointmentAutomaticReminder";
            var jobIdBir = $"{hostName}_Sms_BirthdayAutomaticReminder";
            var jobIdThanksCustomer = $"{hostName}_Sms_ThanksCustomerAutomaticReminder";
            var jobIdCareAfterOrder = $"{hostName}_Sms_CareAfterTreatmentAutomaticReminder";

            if (model.Type == "appointment")
            {
                if (model.IsAppointmentAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdApp, x => x.RunJob(hostName, model.Id), $"*/5 * * * *", TimeZoneInfo.Local);
                }
                else
                {
                    ActionStopJob(jobIdApp);
                }
            }

            if (model.Type == "thanks-customer")
            {
                if (model.IsAppointmentAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdApp, x => x.RunJob(hostName, model.Id), $"*/5 * * * *", TimeZoneInfo.Local);
                }
                else
                {
                    ActionStopJob(jobIdApp);
                }
            }

            if (model.Type == "care-after-order")
            {
                if (model.IsAppointmentAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdApp, x => x.RunJob(hostName, model.Id), $"*/5 * * * *", TimeZoneInfo.Local);
                }
                else
                {
                    ActionStopJob(jobIdApp);
                }
            }

            if (model.Type == "birthday")
            {
                if (model.IsBirthdayAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdBir, x => x.RunJob(hostName, model.Id), $"0 8 * * *", TimeZoneInfo.Local);
                }
                else
                {
                    ActionStopJob(jobIdBir);
                }
            }
        }

        public void ActionStopJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}
