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

            //if (model.Type == "care-after-order")
            //{
            //    if (model.IsCareAfterOrderAutomation)
            //    {
            //        RecurringJob.AddOrUpdate<ISmsJobService>(jobIdCareAfterOrder, x => x.RunJob(hostName, model.Id), $"*/10 * * * *", TimeZoneInfo.Local);
            //    }
            //    else
            //    {
            //        ActionStopJob(jobIdApp);
            //    }
            //}

            if (model.Type == "birthday")
            {
                if (model.IsBirthdayAutomation)
                {
                    RecurringJob.AddOrUpdate<ISmsJobService>(jobIdBir, x => x.RunJob(hostName, model.Id), $"8 * * * *", TimeZoneInfo.Local);
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
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.States))
            {
                var states = val.States.Split(",");
                var stateBools = states.Select(x => bool.Parse(x));
                query = query.Where(x => stateBools.Contains(x.IsCareAfterOrderAutomation));
            }

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
                .Include(x => x.SmsConfigProductRels).FirstOrDefaultAsync();
            entity = _mapper.Map(val, entity);
            if (val.ProductIds.Any())
            {
                entity.SmsConfigProductRels = ComputeProduct(val.ProductIds, entity.SmsConfigProductRels.ToList());
            }
            else
            {
                entity.SmsConfigProductRels = new List<SmsConfigProductRel>();
            }

            if (val.ProductCategoryIds.Any())
            {
                entity.SmsConfigProductCategoryRels = ComputeProductCategory(val.ProductIds, entity.SmsConfigProductCategoryRels.ToList());
            }
            else
            {
                entity.SmsConfigProductCategoryRels = new List<SmsConfigProductCategoryRel>();
            }

            await UpdateAsync(entity);
        }

        public ICollection<SmsConfigProductRel> ComputeProduct(IEnumerable<Guid> productIds, List<SmsConfigProductRel> smsConfigProductRels)
        {
            if (smsConfigProductRels != null && smsConfigProductRels.Any())
            {
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
                if (!smsConfigProductRels.Any(x => x.ProductId == id))
                    smsConfigProductRels.Add(new SmsConfigProductRel
                    {
                        ProductId = id
                    });
            }
            return smsConfigProductRels;
        }

        public ICollection<SmsConfigProductCategoryRel> ComputeProductCategory(IEnumerable<Guid> productIds, List<SmsConfigProductCategoryRel> smsConfigProducCategorytRels)
        {
            if (smsConfigProducCategorytRels != null && smsConfigProducCategorytRels.Any())
            {
                foreach (var item in smsConfigProducCategorytRels)
                {
                    if (!productIds.Any(x => x == item.ProductCategoryId))
                    {
                        smsConfigProducCategorytRels.Remove(item);
                    }
                }
            }

            foreach (var id in productIds)
            {
                if (!smsConfigProducCategorytRels.Any(x => x.ProductCategoryId == id))
                    smsConfigProducCategorytRels.Add(new SmsConfigProductCategoryRel
                    {
                        ProductCategoryId = id
                    });
            }
            return smsConfigProducCategorytRels;
        }
    }
}
