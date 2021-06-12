using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Hangfire;
using Infrastructure.HangfireJobService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsAppointmentAutomationConfigService : BaseService<SmsAppointmentAutomationConfig>, ISmsAppointmentAutomationConfigService
    {
        private readonly AppTenant _tenant;
        private readonly IMapper _mapper;
        public SmsAppointmentAutomationConfigService(IMapper mapper, ITenant<AppTenant> tenant, IAsyncRepository<SmsAppointmentAutomationConfig> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
        }

        public override async Task<SmsAppointmentAutomationConfig> CreateAsync(SmsAppointmentAutomationConfig entity)
        {
            entity = await base.CreateAsync(entity);
            ActionRunJob(entity);
            return entity;
        }

        public async Task UpdateAsync(Guid id, SmsAppointmentAutomationConfigSave val)
        {
            var entity = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            entity = _mapper.Map(val, entity);
            await UpdateAsync(entity);
            ActionRunJob(entity);
        }

        public void ActionRunJob(SmsAppointmentAutomationConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobIdApp = $"{hostName}_Sms_AppointmentAutomaticReminder_{model.Id}";
            if (model.Active)
            {
                RecurringJob.AddOrUpdate<ISmsJobService>(jobIdApp, x => x.RunAppointmentAutomatic(hostName, model.Id), $"*/30 * * * *", TimeZoneInfo.Local);
            }
            else
            {
                ActionStopJob(jobIdApp);
            }
        }

        public void ActionStopJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }

        public async Task<SmsAppointmentAutomationConfigDisplay> GetByCompany(Guid compayId)
        {
            var entity = await SearchQuery(x => x.CompanyId == compayId)
                 .Include(x => x.Template)
                 .Include(x => x.SmsCampaign)
                 .Include(x => x.SmsAccount)
                 .FirstOrDefaultAsync();
            return _mapper.Map<SmsAppointmentAutomationConfigDisplay>(entity);
        }
    }
}
