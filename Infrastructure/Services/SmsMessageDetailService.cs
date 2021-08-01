﻿using ApplicationCore.Entities;
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
using ApplicationCore.Utilities;

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
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);
            return query;
        }

        public async Task<PagedResult2<SmsMessageDetailStatistic>> GetPagedStatistic(SmsMessageDetailPaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageDetailStatistic
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

        public async Task<IEnumerable<SmsMessageDetailReportSummaryResponse>> GetReportTotal(SmsMessageDetailReportSummaryRequest val)
        {
            var query = SearchQuery();
            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= dateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.SmsAccountId.HasValue)
                query = query.Where(x => x.SmsAccountId == val.SmsAccountId.Value);

            if (val.SmsCampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId == val.SmsCampaignId);

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var res = await query.GroupBy(x => x.State)
                .Select(x => new SmsMessageDetailReportSummaryResponse
                {
                    State = x.Key,
                    Total = x.Count(),
                    Percentage = x.Count() * 100f / query.Count()
                }).ToListAsync();
            return res;
        }

        public async Task<IEnumerable<ReportCampaignOutputItem>> GetReportCampaign(ReportCampaignPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.SmsCampaign.Name.Contains(val.Search));

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= dateFrom);
            }    

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var items = await query.Include(x => x.SmsCampaign).ToListAsync();

            var group = items.GroupBy(x => x.SmsCampaignId)
            .Select(y => new 
            {
                CampaignId = y.Key,
                TotalSent = y.Sum(z => z.State == "sent" ? 1 : 0),
                TotalError = y.Sum(z => z.State == "error" ? 1 : 0),
            }).ToList();

            var campaignIds = group.Select(x => x.CampaignId).ToList();
            var campaignObj = GetService<ISmsCampaignService>();
            var campaigns = await campaignObj.SearchQuery(x => campaignIds.Contains(x.Id)).ToListAsync();
            var campaignDict = campaigns.ToDictionary(x => x.Id, x => x);

            var result = group.Select(x => new ReportCampaignOutputItem
            {
                SmsCampaignName = x.CampaignId.HasValue ? campaignDict[x.CampaignId.Value].Name : "Không xác định",
                TotalSuccessfulMessages = x.TotalSent,
                TotalErrorMessages = x.TotalError,
                TotalMessages = x.TotalSent + x.TotalError
            });

            return result;
        }


        public async Task<IEnumerable<ReportSupplierChart>> GetReportSupplierSumary(ReportSupplierPaged val)
        {
            var query = SearchQuery();

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= dateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var items = await query
                .GroupBy(x => new
                {
                    Month = x.Date.Value.Month,
                    Year = x.Date.Value.Year
                }).Select(x => new ReportSupplierChart
                {
                    TotalSent = x.Sum(s => s.State == "sent" ? 1 : 0),
                    TotalError = x.Sum(s => s.State == "error" ? 1 : 0),
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                }).ToListAsync();
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
    }
}
