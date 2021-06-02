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
    public class SmsConfigService : BaseService<SmsConfig>, ISmsConfigService
    {
        private readonly AppTenant _tenant;
        private readonly IMapper _mapper;
        public SmsConfigService(IMapper mapper, ITenant<AppTenant> tenant, IAsyncRepository<SmsConfig> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
        }
        public override async Task<SmsConfig> CreateAsync(SmsConfig entity)
        {
            if (!entity.SmsCampaignId.HasValue && entity.Type == "partner")
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
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "sale-order")
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
            }
            else if (!entity.SmsCampaignId.HasValue && entity.Type == "sale-order-line")
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
            await base.UpdateAsync(entity);
        }

        public async Task<SmsConfigDisplay> GetDisplay(Guid id)
        {
            var entity = await SearchQuery(x => x.Id == id)
                .Include(x => x.Template)
                .Include(x => x.SmsAccount)
                .Include(x => x.SmsConfigProductCategoryRels).ThenInclude(x => x.ProductCategory)
                .Include(x => x.SmsConfigProductRels).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();
            return _mapper.Map<SmsConfigDisplay>(entity);
        }

        public void ActionRunJob(SmsConfig model)
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobIdApp = $"{hostName}_Sms_AppointmentAutomaticReminder";
            var jobIdBir = $"{hostName}_Sms_BirthdayAutomaticReminder";
            var jobIdCareAfterOrder = $"{hostName}_Sms_CareAfterTreatmentAutomaticReminder_{model.Id}";

            if (model.Type == "appointment")
            {
                if (model.IsAppointmentAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdApp, x => x.RunAppointmentAutomatic(hostName, model.Id), $"*/30 * * * *", TimeZoneInfo.Local);
                }
                else
                {
                    ActionStopJob(jobIdApp);
                }
            }

            if (model.Type == "sale-order-line")
            {
                if (model.IsCareAfterOrderAutomation && model.DateSend.HasValue)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdCareAfterOrder, x => x.RunCareAfterOrderAutomatic(hostName, model.Id), $"{model.DateSend.Value.Minute} {model.DateSend.Value.Hour} * * *", TimeZoneInfo.Local);
                }
                else
                {
                    ActionStopJob(jobIdCareAfterOrder);
                }
            }

            if (model.Type == "partner" && model.DateSend.HasValue)
            {
                if (model.IsBirthdayAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdBir, x => x.RunBirthdayAutomatic(hostName, model.Id), $"{model.DateSend.Value.Minute} {model.DateSend.Value.Hour} * * *", TimeZoneInfo.Local);
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

        public async Task<PagedResult2<SmsConfigGrid>> GetPaged(SmsConfigPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.States))
            {
                var states = val.States.Split(",");
                var stateBools = states.Select(x => bool.Parse(x));
                query = query.Where(x => stateBools.Contains(x.IsCareAfterOrderAutomation));
            }

            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type == val.Type);

            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).OrderByDescending(x => x.DateCreated).Select(x => new SmsConfigGrid
            {
                Id = x.Id,
                Name = x.Name,
                Date = x.DateSend,
                BrandName = $"{x.SmsAccount.BrandName} ({x.SmsAccount.Name})",
                IsAppointmentAutomation = x.IsAppointmentAutomation,
                IsBirthdayAutomation = x.IsBirthdayAutomation,
                IsCareAfterOrderAutomation = x.IsCareAfterOrderAutomation,
                IsThanksCustomerAutomation = x.IsThanksCustomerAutomation,
                TimeBeforSend = x.TimeBeforSend,
                TypeTimeBeforSend = x.TypeTimeBeforSend,
                ProductNames = x.SmsConfigProductRels != null && x.SmsConfigProductRels.Any() ? string.Join(", ", x.SmsConfigProductRels.Select(x => x.Product.Name)) : null,
                ProductCategoryNames = x.SmsConfigProductCategoryRels != null && x.SmsConfigProductCategoryRels.Any() ? string.Join(", ", x.SmsConfigProductCategoryRels.Select(x => x.ProductCategory.Name)) : null,

            }).ToListAsync();
            return new PagedResult2<SmsConfigGrid>(totalItems, val.Offset, val.Limit) { Items = items };
        }

        public async Task UpdateAsync(Guid id, SmsConfigSave val)
        {
            var entity = await SearchQuery(x => x.Id == id)
                .Include(x => x.SmsConfigProductRels)
                .Include(x => x.SmsConfigProductCategoryRels).FirstOrDefaultAsync();
            entity = _mapper.Map(val, entity);
            if (val.ProductIds.Any())
            {
                entity.SmsConfigProductRels = ComputeProduct(val.ProductIds, entity);
            }
            else
            {
                entity.SmsConfigProductRels = new List<SmsConfigProductRel>();
            }

            if (val.ProductCategoryIds.Any())
            {
                entity.SmsConfigProductCategoryRels = ComputeProductCategory(val.ProductCategoryIds, entity);
            }
            else
            {
                entity.SmsConfigProductCategoryRels = new List<SmsConfigProductCategoryRel>();
            }

            await UpdateAsync(entity);
        }

        public ICollection<SmsConfigProductRel> ComputeProduct(IEnumerable<Guid> productIds, SmsConfig smsConfig)
        {
            if (smsConfig.SmsConfigProductRels != null && smsConfig.SmsConfigProductRels.Any())
            {
                var smsConfigProductRels = smsConfig.SmsConfigProductRels.ToList();
                for (int i = 0; i < smsConfigProductRels.Count(); i++)
                {
                    var item = smsConfigProductRels[i];
                    if (!productIds.Any(x => x == item.ProductId))
                    {
                        smsConfigProductRels.Remove(item);
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

        public ICollection<SmsConfigProductCategoryRel> ComputeProductCategory(IEnumerable<Guid> ProductCategoryIds, SmsConfig smsConfig)
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
    }
}
