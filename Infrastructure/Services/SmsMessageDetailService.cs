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
    public class SmsMessageDetailService : BaseService<SmsMessageDetail>, ISmsMessageDetailService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;

        public SmsMessageDetailService(ITenant<AppTenant> tenant, IAsyncRepository<SmsMessageDetail> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
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
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateFrom.Value <= x.DateCreated.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateTo.Value >= x.DateCreated.Value);
            
            var totalItems = await query.CountAsync();
            var items = await query.Include(x => x.SmsAccount).Include(x => x.Partner).OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsMessageDetailBasic>(totalItems: totalItems, limit: val.Limit, offset: val.Offset)
            {
                Items = _mapper.Map<IEnumerable<SmsMessageDetailBasic>>(items)
            };
        }

        public async Task RunJobSendSms()
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{hostName}_Send_Sms_message_detail";
            RecurringJob.AddOrUpdate<ISmsMessageJobService>(jobId, x => x.RunJobFindSmsMessage(hostName, CompanyId), $"*/5 * * * *", TimeZoneInfo.Local);
        }
    }
}
