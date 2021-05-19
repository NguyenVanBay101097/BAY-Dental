using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsMessageService : BaseService<SmsMessage>, ISmsMessageService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly AppTenant _tenant;
        public SmsMessageService(IConfiguration configuration, ITenant<AppTenant> tenant, IMapper mapper, IAsyncRepository<SmsMessage> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
            _configuration = configuration;
        }

        private IQueryable<SmsMessage> GetQueryable(SmsMessagePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);
            if (val.CampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId.Value == val.CampaignId.Value);
            return query;
        }

        public async Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageBasic
            {
                Id = x.Id,
                Date = x.Date,
                DateCreated = x.DateCreated,
                Name = x.Name,
                CountPartner = x.Partners.Count(),
                BrandName = x.SmsAccountId.HasValue ? x.SmsAccount.BrandName + $" ({x.SmsAccount.Name})" : "",
            }).ToListAsync();
            return new PagedResult2<SmsMessageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SmsMessageDisplay> CreateAsync(SmsMessageSave val)
        {
            var entity = _mapper.Map<SmsMessage>(val);
            if (entity.TypeSend == "manual")
                entity.State = "sending";
            else
            {
                entity.State = "waiting";
            }

            if (!entity.SmsCampaignId.HasValue && val.IsBirthDayManual.HasValue && val.IsBirthDayManual.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignBirthday();
                entity.SmsCampaignId = campaign.Id;
            }

            else if (!entity.SmsCampaignId.HasValue && val.IsAppointmentReminder.HasValue && val.IsAppointmentReminder.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignAppointmentReminder();
                entity.SmsCampaignId = campaign.Id;
            }

            else if (!entity.SmsCampaignId.HasValue)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaign();
                entity.SmsCampaignId = campaign.Id;
            }

            if (val.PartnerIds.Any())
            {
                foreach (var partnerId in val.PartnerIds)
                {
                    entity.Partners.Add(new SmsMessagePartnerRel()
                    {
                        PartnerId = partnerId
                    });
                }
            }

            entity = await CreateAsync(entity);
            return _mapper.Map<SmsMessageDisplay>(entity);
        }

        public async Task ActionSendSMS(SmsMessage entity)
        {
            var smsSendMessageObj = GetService<ISmsSendMessageService>();
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            await using var context = DbContextHelper.GetCatalogDbContext(hostName, _configuration);
            try
            {
                var partnerIds = entity.Partners.Select(x => x.PartnerId).ToList();
                if (partnerIds != null && partnerIds.Any())
                {
                    var companyId = CompanyId;
                    await smsSendMessageObj.CreateSmsMessageDetail(context, entity, partnerIds, companyId);
                    entity.State = "success";
                }
                await UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ActionCancel(IEnumerable<Guid> messIds)
        {
            var messes = await SearchQuery(x => messIds.Contains(x.Id)).ToListAsync();
            if (messes.Any())
            {
                await DeleteAsync(messes);
            }
        }
    }
}
