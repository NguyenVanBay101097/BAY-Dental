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

            if (!string.IsNullOrEmpty(val.States))
            {
                var states = val.States.Split(",");
                var stateBools = states.Select(x => bool.Parse(x));
                query = query.Where(x => stateBools.Contains(x.Active));
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
            var entity = await SearchQuery(x => x.Id == id)
                 .Include(x => x.SmsConfigProductRels)
                 .Include(x => x.SmsConfigProductCategoryRels).FirstOrDefaultAsync();
            entity = _mapper.Map(val, entity);
            if (entity.ApplyOn == "product")
            {
                entity.SmsConfigProductRels = ComputeProduct(val.ProductIds, entity);
            }
            else if (entity.ApplyOn == "product_category")
            {
                entity.SmsConfigProductCategoryRels = ComputeProductCategory(val.ProductCategoryIds, entity);
            }
         
            await UpdateAsync(entity);
            ActionRunJob(entity);
        }

        public ICollection<SmsConfigProductRel> ComputeProduct(IEnumerable<Guid> productIds, SmsCareAfterOrderAutomationConfig smsConfig)
        {
            if (smsConfig.SmsConfigProductRels != null && smsConfig.SmsConfigProductRels.Any())
            {
                var smsConfigProductRels = smsConfig.SmsConfigProductRels.ToList();
                for (int i = 0; i < smsConfigProductRels.Count(); i++)
                {
                    var item = smsConfigProductRels[i];
                    if (!productIds.Any(x => x == item.ProductId))
                    {
                        smsConfig.SmsConfigProductRels.Remove(item);
                    }
                }
            }

            foreach (var id in productIds)
            {
                if (!smsConfig.SmsConfigProductRels.Any(x => x.ProductId == id))
                    smsConfig.SmsConfigProductRels.Add(new SmsConfigProductRel
                    {
                        ProductId = id,
                        SmsConfigId = smsConfig.Id
                    });
            }
            return smsConfig.SmsConfigProductRels;
        }

        public ICollection<SmsConfigProductCategoryRel> ComputeProductCategory(IEnumerable<Guid> ProductCategoryIds, SmsCareAfterOrderAutomationConfig smsConfig)
        {
            if (smsConfig.SmsConfigProductCategoryRels != null && smsConfig.SmsConfigProductCategoryRels.Any())
            {
                var smsConfigProductCategoryRels = smsConfig.SmsConfigProductCategoryRels.ToList();
                foreach (var item in smsConfigProductCategoryRels)
                {
                    if (!ProductCategoryIds.Any(x => x == item.ProductCategoryId))
                    {
                        smsConfigProductCategoryRels.Remove(item);
                    }
                }
            }

            foreach (var id in ProductCategoryIds)
            {
                if (!smsConfig.SmsConfigProductCategoryRels.Any(x => x.ProductCategoryId == id))
                    smsConfig.SmsConfigProductCategoryRels.Add(new SmsConfigProductCategoryRel
                    {
                        ProductCategoryId = id,
                        SmsConfigId = smsConfig.Id
                    });
            }
            return smsConfig.SmsConfigProductCategoryRels;
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
