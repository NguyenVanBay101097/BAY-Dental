﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ApplicationCore.Utilities;
using Dapper;

namespace Infrastructure.Services
{
    public class CashBookService : ICashBookService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashBookService(CatalogDbContext context, IMapper mapper,
           IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
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

        public async Task<decimal> GetTotal(CashBookSearch val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var cashBookReport = new CashBookReport();

            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType == "liquidity");
            var total = await query.SumAsync(x => x.Debit - x.Credit);
            return total;
        }

        public async Task<CashBookReport> GetSumary(CashBookSearch val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var cashBookReport = new CashBookReport();

            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: val.CompanyId, initBal: true);
                query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                cashBookReport.Begin = begin;
            }

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
            cashBookReport.TotalThu = await query.SumAsync(x => x.Credit);
            cashBookReport.TotalChi = await query.SumAsync(x => x.Debit);

            cashBookReport.TotalAmount = cashBookReport.Begin + cashBookReport.TotalThu - cashBookReport.TotalChi;
            return cashBookReport;
        }

        public async Task<PagedResult2<CashBookReportDetail>> GetDetails(CashBookDetailFilter val)
        {
            var amlObj = GetService<IAccountMoveLineService>();

            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Move.InvoiceOrigin.Contains(val.Search));

            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit).Select(x => new CashBookReportDetail
            {
                AccountName = x.Account.Name,
                Amount = x.Credit - x.Debit,
                Date = x.Date,
                JournalName = x.Journal.Name,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                InvoiceOrigin = x.Move.InvoiceOrigin
            }).ToListAsync();

            return new PagedResult2<CashBookReportDetail>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<CashBookReportItem>> GetChartReport(CashBookReportFilter val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var res = new List<CashBookReportItem>();

            var types = new string[] { "cash", "bank" };

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");

            if (val.GroupBy == "groupby:day")
            {

                res = await query.GroupBy(x => x.Date.Value.Date)
                  .Select(x => new CashBookReportItem
                  {
                      Date = x.Key,
                      Begin = x.Sum(s => s.Credit - s.Debit),
                      TotalThu = x.Sum(s => s.Credit),
                      TotalChi = x.Sum(s => s.Debit),
                  }).ToListAsync();



            }
            else if (val.GroupBy == "groupby:month")
            {
                res = await query.GroupBy(x => new
                {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                })
                 .Select(x => new CashBookReportItem
                 {
                     Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                     Begin = x.Sum(s => s.Credit - s.Debit),
                     TotalThu = x.Sum(s => s.Credit),
                     TotalChi = x.Sum(s => s.Debit),
                 }).ToListAsync();

            }

            foreach (var item in res)
                item.TotalAmount = item.Begin + item.TotalThu - item.TotalChi;

            return res;
        }
    }
}
