using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Hangfire;
using Infrastructure.HangfireJobService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsBirthdayAutomationConfigService : BaseService<SmsBirthdayAutomationConfig>, ISmsBirthdayAutomationConfigService
    {
        private readonly AppTenant _tenant;
        private readonly IMapper _mapper;
        public SmsBirthdayAutomationConfigService(IMapper mapper, ITenant<AppTenant> tenant, IAsyncRepository<SmsBirthdayAutomationConfig> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
        }

        public async Task<SmsBirthdayAutomationConfigDisplay> GetDisplay(Guid id)
        {
            var entity = await SearchQuery(x => x.Id == id)
                .Include(x => x.Template)
                .Include(x => x.SmsCampaign)
                .Include(x => x.SmsAccount)
                .FirstOrDefaultAsync();
            return _mapper.Map<SmsBirthdayAutomationConfigDisplay>(entity);
        }

        public override async Task<SmsBirthdayAutomationConfig> CreateAsync(SmsBirthdayAutomationConfig entity)
        {
            entity = await base.CreateAsync(entity);
            ActionRunJob(entity);
            return entity;
        }

        public async Task UpdateAsync(Guid id, SmsBirthdayAutomationConfigSave val)
        {
            var entity = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            entity = _mapper.Map(val, entity);
            await UpdateAsync(entity);
            ActionRunJob(entity);
        }

        public void ActionRunJob(SmsBirthdayAutomationConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobIdBir = $"{hostName}_Sms_BirthdayAutomaticReminder_{model.CompanyId}";
            if (model.Active)
            {
                RecurringJob.AddOrUpdate<ISmsJobService>(jobIdBir, x => x.RunBirthdayAutomatic(hostName, model.CompanyId.Value), $"{model.ScheduleTime.Value.Minute} {model.ScheduleTime.Value.Hour} * * *", TimeZoneInfo.Local);
            }
            else
            {
                ActionStopJob(jobIdBir);
            }
        }

        public void ActionStopJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }

        public async Task<SmsBirthdayAutomationConfigDisplay> GetByCompany(Guid companyId)
        {
            var entity = await SearchQuery(x => x.CompanyId == companyId)
              .Include(x => x.Template)
              .Include(x => x.SmsCampaign)
              .Include(x => x.SmsAccount)
              .FirstOrDefaultAsync();
            return _mapper.Map<SmsBirthdayAutomationConfigDisplay>(entity);
        }
    }
}
