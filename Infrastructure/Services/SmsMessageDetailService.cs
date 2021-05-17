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

        public async Task<PagedResult2<SmsMessageDetailBasic>> GetPaged(SmsMessageDetailPaged val)
        {
            var query = SearchQuery();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Number.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State));
            if (val.SmsCampaignId.HasValue)
                query = query.Where(x => x.SmsMessage.SmsCampaignId.HasValue && x.SmsMessage.SmsCampaignId == val.SmsCampaignId.Value);
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
