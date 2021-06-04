using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.HangfireJobService;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyERP.Utilities;
using Newtonsoft.Json;
using RestSharp;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsMessageDetailService : ISmsMessageDetailService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly IAsyncRepository<SmsMessageDetail> _repository;
        private readonly ISmsMessageService _smsMessageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CatalogDbContext _context;
        public SmsMessageDetailService(
            IHttpContextAccessor httpContextAccessor,
            ITenant<AppTenant> tenant,
            ISmsMessageService smsMessageService,
            IAsyncRepository<SmsMessageDetail> repository,
            CatalogDbContext context,
            IMapper mapper)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
            _smsMessageService = smsMessageService;
            _context = context;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<SmsMessageDetail> GetQueryable(SmsMessageDetailPaged val)
        {
            var query = _repository.SearchQuery();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Number.Contains(val.Search) ||
                (x.PartnerId.HasValue && (x.Partner.Name.Contains(val.Search) || (x.Partner.NameNoSign.Contains(val.Search)))) ||
                (x.SmsMessageId.HasValue && x.SmsMessage.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State));
            if (val.SmsCampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == val.SmsCampaignId.Value);
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
                Date = x.Date,
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
            var smsAccountObj = GetService<ISmsAccountService>();
            var smsMessageObj = GetService<ISmsMessageService>();
            var dictSmsAccount = await smsAccountObj.SearchQuery().ToDictionaryAsync(x => x.Id, x => x);
            var dictSmsDetails = details.GroupBy(x => x.SmsAccountId).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var item in dictSmsDetails)
            {
                if (dictSmsAccount.ContainsKey(item.Key))
                {
                    await smsMessageObj.ActionSendSmsMessageDetail(item.Value, dictSmsAccount[item.Key]);
                }
            }

        }


        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public IQueryable<SmsMessageDetail> SearchQuery()
        {
            var query = _repository.SearchQuery();
            return query;
        }

        public async Task<IEnumerable<ReportTotalOutputItem>> GetReportTotal(ReportTotalInput val)
        {
            var query = SearchQuery();
            if (val.Date.HasValue)
                query = query.Where(x => x.Date.Value.Month == val.Date.Value.Month);
            if (val.SmsAccountId.HasValue)
                query = query.Where(x => x.SmsAccountId == val.SmsAccountId.Value);
            if (val.SmsCampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId.Value == val.SmsCampaignId.Value);

            var res = await query.GroupBy(x => new { x.State })
                .Select(x => new ReportTotalOutputItem
                {
                    State = x.Key.State,
                    StateDisplay = x.Key.State == "sent" ? "Thành công" : (x.Key.State == "canceled" ? "Hủy" : (x.Key.State == "error" ? "Thất bại" : "Đang gửi")),
                    Total = x.Count(),
                    Percentage = x.Count() * 100f / query.Count()
                }).ToListAsync();
            return res;
        }

        public async Task<PagedResult2<ReportCampaignOutputItem>> GetReportCampaign(ReportCampaignPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.SmsCampaign.Name.Contains(val.Search) || (x.SmsCampaignId.HasValue && x.SmsCampaign.Name.Contains(val.Search)));
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date.HasValue && val.DateFrom.Value <= x.Date.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date.HasValue && val.DateTo.Value >= x.Date.Value);

            var items = await query.Include(x => x.SmsCampaign).ToListAsync();

            var itemsOutput = items.GroupBy(x => x.SmsCampaignId)
            .Select(y => new ReportCampaignOutputItem
            {
                SmsCampaignName = y.First().SmsCampaignId.HasValue ? y.First().SmsCampaign.Name : "",
                TotalMessages = y.Count(),
                TotalSuccessfulMessages = y.Count(z => z.State == "sent"),
                TotalCancelMessages = y.Count(z => z.State == "canceled"),
                TotalOutgoingdMessages = y.Count(z => z.State == "outgoing"),
                TotalErrorMessages = y.Count(z => z.State == "error")
            }).ToList();

            var totalItems = itemsOutput.Count();
            var itemsResult = itemsOutput.Skip(val.Offset).Take(val.Limit).ToList();

            return new PagedResult2<ReportCampaignOutputItem>(totalItems: totalItems, limit: val.Limit, offset: val.Offset)
            {
                Items = itemsResult
            };
        }


        public async Task<IEnumerable<ReportSupplierChart>> GetReportSupplierSumary(ReportSupplierPaged val)
        {
            var total = await SearchQuery().CountAsync();
            var query = SearchQuery().Where(x => x.Date.HasValue);
            if (!string.IsNullOrEmpty(val.Provider))
                query = query.Where(x => x.SmsAccount.Provider == val.Provider && x.Date.HasValue);
            if (!string.IsNullOrEmpty(val.State))
            {
                query = query.Where(x => x.State == val.State);
            }
            if (val.AccountId.HasValue)
                query = query.Where(x => x.SmsAccountId == val.AccountId.Value);

            var items = await query
                .GroupBy(x => new
                {
                    Month = x.Date.Value.Month,
                    Year = x.Date.Value.Year
                }).Select(x => new ReportSupplierChart
                {
                    StateName = val.State == "sent" ? "Thành công" : (val.State == "canceled" ? "Hủy" : (val.State == "error" ? "Thất bại" : "Đang gửi")),
                    Color = val.State == "sent" ? "#007bff" : (val.State == "canceled" ? "#ff0000" : (val.State == "error" ? "#ffab00" : "#020202")),
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    Count = x.Count(),
                    Total = total
                }).OrderBy(x=>x.Month).ToListAsync();
            return items;
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        //public override ISpecification<SmsMessageDetail> RuleDomainGet(IRRule rule)
        //{
        //    switch (rule.Code)
        //    {
        //        case "sms.sms_campaign_comp_rule":
        //            return new InitialSpecification<SmsMessageDetail>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
        //        default:
        //            return null;
        //    }
        //}
    }
}
