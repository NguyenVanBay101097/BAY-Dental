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
    public class SmsCareAfterOrderAutomationConfigService : BaseService<SmsCareAfterOrderAutomationConfig>, ISmsCareAfterOrderAutomationConfigService
    {
        private readonly AppTenant _tenant;
        private readonly IMapper _mapper;
        public SmsCareAfterOrderAutomationConfigService(IMapper mapper, ITenant<AppTenant> tenant, IAsyncRepository<SmsCareAfterOrderAutomationConfig> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
        }

        public async Task<SmsCareAfterOrderAutomationConfigDisplay> GetDisplay(Guid id)
        {
            var entity = await SearchQuery(x => x.Id == id)
                .Include(x => x.Template)
                .Include(x => x.SmsCampaign)
                .Include(x => x.SmsAccount)
                .Include(x => x.SmsConfigProductCategoryRels).ThenInclude(x => x.ProductCategory)
                .Include(x => x.SmsConfigProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();
            return _mapper.Map<SmsCareAfterOrderAutomationConfigDisplay>(entity);
        }

        public async Task<PagedResult2<SmsCareAfterOrderAutomationConfigGrid>> GetPaged(SmsCareAfterOrderAutomationConfigPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.Active.HasValue)
            {
                query = query.Where(x => x.Active == val.Active.Value);
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).OrderByDescending(x => x.DateCreated).Select(x => new SmsCareAfterOrderAutomationConfigGrid
            {
                Id = x.Id,
                Name = x.Name,
                ScheduleTime = x.ScheduleTime,
                BrandName = $"{x.SmsAccount.BrandName} ({x.SmsAccount.Name})",
                Active = x.Active,
                TimeBeforSend = x.TimeBeforSend,
                TypeTimeBeforSend = x.TypeTimeBeforSend,
                ProductNames = x.SmsConfigProductRels != null && x.SmsConfigProductRels.Any() ? string.Join(", ", x.SmsConfigProductRels.Select(x => x.Product.Name)) : null,
                ProductCategoryNames = x.SmsConfigProductCategoryRels != null && x.SmsConfigProductCategoryRels.Any() ? string.Join(", ", x.SmsConfigProductCategoryRels.Select(x => x.ProductCategory.Name)) : null,

            }).ToListAsync();
            return new PagedResult2<SmsCareAfterOrderAutomationConfigGrid>(totalItems, val.Offset, val.Limit) { Items = items };
        }

        public override async Task<SmsCareAfterOrderAutomationConfig> CreateAsync(SmsCareAfterOrderAutomationConfig entity)
        {
            entity = await base.CreateAsync(entity);
            ActionRunJob(entity);
            return entity;
        }

        public async Task UpdateAsync(Guid id, SmsCareAfterOrderAutomationConfigSave val)
        {
            throw new NotImplementedException();
        }

        public void ActionRunJob(SmsCareAfterOrderAutomationConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobIdCareAfterOrder = $"{hostName}_Sms_CareAfterTreatmentAutomaticReminder_{model.Id}";


            if (model.Active && model.ScheduleTime.HasValue)
            {
                RecurringJob.AddOrUpdate<ISmsJobService>(jobIdCareAfterOrder, x => x.RunCareAfterOrderAutomatic(hostName, model.Id), $"{model.ScheduleTime.Value.Minute} {model.ScheduleTime.Value.Hour} * * *", TimeZoneInfo.Local);
            }
            else
            {
                ActionStopJob(jobIdCareAfterOrder);
            }

        }

        public void ActionStopJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}
