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

            return query;
        }

        public async Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsMessageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SmsMessageBasic>>(items)
            };
        }

        public async Task<SmsMessageBasic> CreateAsync(SmsMessageSave val)
        {
            var entity = _mapper.Map<SmsMessage>(val);
            if (entity.TypeSend == "manual")
                entity.State = "sending";

            if (!entity.SmsCampaignId.HasValue && val.IsBirthDayManual.HasValue && val.IsBirthDayManual.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignBirthday();
                entity.SmsCampaignId = campaign.Id;
            }

            if (!entity.SmsCampaignId.HasValue && val.IsAppointmentReminder.HasValue && val.IsAppointmentReminder.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignAppointmentReminder();
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
            return _mapper.Map<SmsMessageBasic>(entity);
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
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
