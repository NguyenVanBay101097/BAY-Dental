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
    public class SmsThanksCustomerAutomationConfigService : BaseService<SmsThanksCustomerAutomationConfig>, ISmsThanksCustomerAutomationConfigService
    {
        private readonly AppTenant _tenant;
        private readonly IMapper _mapper;
        public SmsThanksCustomerAutomationConfigService(IMapper mapper, ITenant<AppTenant> tenant, IAsyncRepository<SmsThanksCustomerAutomationConfig> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
        }

        public async Task<SmsThanksCustomerAutomationConfigDisplay> GetByCompany()
        {
            var entity = await SearchQuery(x => x.CompanyId == CompanyId)
                .Include(x => x.Template)
                .Include(x => x.SmsCampaign)
                .Include(x => x.SmsAccount)
                .FirstOrDefaultAsync();
            return _mapper.Map<SmsThanksCustomerAutomationConfigDisplay>(entity);
        }

        public override async Task<SmsThanksCustomerAutomationConfig> CreateAsync(SmsThanksCustomerAutomationConfig entity)
        {
            entity = await base.CreateAsync(entity);
            ActionSetupJob(entity);
            return entity;
        }

        public override async Task UpdateAsync(SmsThanksCustomerAutomationConfig entity)
        {
            await base.UpdateAsync(entity);
            ActionSetupJob(entity);
        }

        public void ActionSetupJob(SmsThanksCustomerAutomationConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{hostName}_Sms_ThankCustomerAutomaticReminder_{model.CompanyId}";

            if (model.Active)
            {
                RecurringJob.AddOrUpdate<ISmsJobService>(jobId, x => x.RunThankCustomerAutomatic(hostName, model.CompanyId.Value), $"*/30 * * * *", TimeZoneInfo.Local);
            }
            else
            {
                RecurringJob.RemoveIfExists(jobId);
            }
        }
    }
}
