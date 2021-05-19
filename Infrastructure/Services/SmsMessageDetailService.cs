using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Hangfire;
using Infrastructure.HangfireJobService;
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
    public class SmsMessageDetailService : BaseService<SmsMessageDetail>, ISmsMessageDetailService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly IConfiguration _configuration;

        public SmsMessageDetailService(IConfiguration configuration, ITenant<AppTenant> tenant, IAsyncRepository<SmsMessageDetail> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _configuration = configuration;
            _tenant = tenant?.Value;
        }

        public IQueryable<SmsMessageDetail> GetQueryable(SmsMessageDetailPaged val)
        {
            var query = SearchQuery();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Number.Contains(val.Search) ||
                (x.PartnerId.HasValue && (x.Partner.Name.Contains(val.Search) || (x.Partner.NameNoSign.Contains(val.Search)))) ||
                (x.SmsMessageId.HasValue && x.SmsMessage.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State));
            if (val.SmsCampaignId.HasValue)
                query = query.Where(x => x.SmsMessage.SmsCampaignId.HasValue && x.SmsMessage.SmsCampaignId == val.SmsCampaignId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateFrom.Value <= x.DateCreated.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateTo.Value >= x.DateCreated.Value);
            if (val.SmsMessageId.HasValue)
                query = query.Where(x => x.SmsMessageId.HasValue && x.SmsMessageId.Value == val.SmsMessageId.Value);
            return query;
        }

        public async Task<PagedResult2<SmsMessageDetailStatistic>> GetPagedStatistic(SmsMessageDetailPaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageDetailStatistic
            {
                Id = x.Id,
                Body = x.Body,
                BrandName = x.SmsAccount.BrandName,
                DateCreated = x.DateCreated,
                ErrorCode = x.ErrorCode,
                Number = x.Number,
                PartnerName = x.Partner.DisplayName,
                SmsCampaignName = x.SmsCampaignId.HasValue ? x.SmsCampaign.Name : "",
                SmsMessageName = x.SmsMessageId.HasValue ? x.SmsMessage.Name : "",
                State = x.State
            }).ToListAsync();

            return new PagedResult2<SmsMessageDetailStatistic>(totalItems: totalItems, offset: val.Offset, limit: val.Limit)
            {
                Items = items
            };
        }

        public async Task<PagedResult2<SmsMessageDetailBasic>> GetPaged(SmsMessageDetailPaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Include(x => x.SmsAccount).Include(x => x.Partner).OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsMessageDetailBasic>(totalItems: totalItems, limit: val.Limit, offset: val.Offset)
            {
                Items = _mapper.Map<IEnumerable<SmsMessageDetailBasic>>(items)
            };
        }

        public async Task ReSendSms(IEnumerable<SmsMessageDetail> details)
        {
            var smsSendMessageObj = GetService<ISmsSendMessageService>();
            var smsAccountObj = GetService<ISmsAccountService>();
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            await using var context = DbContextHelper.GetCatalogDbContext(hostName, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var dictSmsAccount = await smsAccountObj.SearchQuery().ToDictionaryAsync(x => x.Id, x => x);
                var smsMessageDetailGroup = details.GroupBy(x => x.SmsAccountId).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var key in smsMessageDetailGroup.Keys)
                {
                    if (dictSmsAccount.ContainsKey(key))
                    {
                        var dict = await smsSendMessageObj.SendSMS(smsMessageDetailGroup[key], dictSmsAccount[key], context);
                        foreach (var item in smsMessageDetailGroup[key])
                        {
                            if (dict.ContainsKey(item.Id))
                            {
                                item.State = dict[item.Id].Message;
                                item.ErrorCode = dict[item.Id].CodeResult;
                                context.Entry(item).State = EntityState.Modified;
                            }
                        }
                        await context.SaveChangesAsync();
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task RunJobSendSms()
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{hostName}_Send_Sms_message_detail";
            RecurringJob.AddOrUpdate<ISmsMessageJobService>(jobId, x => x.RunJobFindSmsMessage(hostName, CompanyId), $"*/5 * * * *", TimeZoneInfo.Local);
        }
    }
}
