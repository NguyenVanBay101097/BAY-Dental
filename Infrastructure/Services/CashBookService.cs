using ApplicationCore.Entities;
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

        public async Task<CashBookReport> GetSumary(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string resultSelection, Guid? journalId)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var cashBookReport = new CashBookReport();

            var types = new string[] { "cash", "bank" };
            if (resultSelection == "cash")
                types = new string[] { "cash" };
            else if (resultSelection == "bank")
                types = new string[] { "bank" };

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: companyId, initBal: true);
                query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                if (journalId.HasValue)
                    query1 = query1.Where(x => x.JournalId == journalId);
                var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                cashBookReport.Begin = begin;
            }

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
            if (journalId.HasValue)
                query = query.Where(x => x.JournalId == journalId.Value);

            cashBookReport.TotalThu = await query.SumAsync(x => x.Credit);
            cashBookReport.TotalChi = await query.SumAsync(x => x.Debit);

            cashBookReport.TotalAmount = cashBookReport.Begin + cashBookReport.TotalThu - cashBookReport.TotalChi;
            return cashBookReport;
        }

        public async Task<CashBookReport> GetSumaryDayReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string resultSelection, Guid? journalId)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var cashBookReport = new CashBookReport();

            var types = new string[] { "cash", "bank" };
            if (resultSelection == "cash")
                types = new string[] { "cash" };
            else if (resultSelection == "bank")
                types = new string[] { "bank" };

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: companyId, initBal: true);
                query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                if (journalId.HasValue)
                    query1 = query1.Where(x => x.JournalId == journalId);
                var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                cashBookReport.Begin = begin;
            }


            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");

            if (journalId.HasValue)
                query = query.Where(x => x.JournalId == journalId);
           
            cashBookReport.TotalThu = await query.SumAsync(x => x.Credit);
            cashBookReport.TotalChi = await query.SumAsync(x => x.Debit);

            cashBookReport.TotalAmount = cashBookReport.Begin + cashBookReport.TotalThu - cashBookReport.TotalChi;
            return cashBookReport;
        }

        public async Task<SumaryCashBook> GetSumaryCashBookReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string partnerType, string accountCode, string resultSelection)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var SumaryCashBook = new SumaryCashBook();

            var types = new string[] { };
            if (resultSelection == "cash_bank")
                types = new string[] { "cash", "bank" };
            else if (resultSelection == "debt")
                types = new string[] { "debt" };
            else if (resultSelection == "advance")
                types = new string[] { "advance" };
            else if (resultSelection == "payroll")
                types = new string[] { "payroll" };
            else if (resultSelection == "commission")
                types = new string[] { };

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);

            if (types.Any())
                query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");

            if (!string.IsNullOrEmpty(partnerType))
                query = query.Where(x => partnerType == "customer" ? x.Partner.Customer : (partnerType == "supplier" ? x.Partner.Supplier : x.Partner.IsAgent));

            if (!string.IsNullOrEmpty(accountCode))
                query = query.Where(x => x.Account.Code == accountCode);

            SumaryCashBook.Type = resultSelection;
            SumaryCashBook.Credit = await query.SumAsync(x => x.Credit);
            SumaryCashBook.Debit = await query.SumAsync(x => x.Debit);
            SumaryCashBook.Balance = await query.SumAsync(x => x.Balance);
            return SumaryCashBook;
        }

        public async Task<PagedResult2<CashBookReportDetail>> GetDetails(DateTime? dateFrom,
            DateTime? dateTo,
            int limit,
            int offset,
            Guid? companyId,
            string search,
            string resultSelection,
            Guid? journalId,
            IEnumerable<Guid> accountIds = null,
            string paymentType = "all")
        {
            var amlObj = GetService<IAccountMoveLineService>();

            var types = new string[] { "cash", "bank" };
            if (resultSelection == "cash")
                types = new string[] { "cash" };
            else if (resultSelection == "bank")
                types = new string[] { "bank" };

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Move.InvoiceOrigin.Contains(search) || x.Partner.Name.Contains(search) || x.Partner.NameNoSign.Contains(search) ||
                    x.Partner.Ref.Contains(search));

            if (journalId.HasValue)
                query = query.Where(x => x.JournalId == journalId);

            if (accountIds != null)
            {
                if (accountIds.Any())
                    query = query.Where(x => accountIds.Contains(x.AccountId));
            }

            if (!string.IsNullOrWhiteSpace(paymentType))
            {
                if (paymentType == "inbound")
                {
                    query = query.Where(x => x.Credit > 0);
                }
                else if (paymentType == "outbound")
                {
                    query = query.Where(x => x.Debit > 0);
                }
            }

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.Date).ThenByDescending(x => x.DateCreated);
            if (limit > 0)
                query = query.Skip(offset).Take(limit);

            var items = await query.Select(x => new CashBookReportDetail
            {
                AccountName = x.Account.Name,
                Amount = x.Credit - x.Debit,
                Date = x.Date,
                JournalName = x.Journal.Name,
                JournalType = x.Journal.Type,
                Name = x.Name,
                PartnerName = x.Partner.Name,
                InvoiceOrigin = x.Move.InvoiceOrigin
            }).ToListAsync();

            return new PagedResult2<CashBookReportDetail>(totalItems, offset, limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<CashBookReportItem>> GetCashBookChartReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var res = new List<CashBookReportItem>();

            var types = new string[] { "cash", "bank" };

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
            query = query.OrderBy(x => x.Date);

            if (groupBy == "groupby:day")
            {

                res = await query.GroupBy(x => x.Date.Value.Date)
                  .Select(x => new CashBookReportItem
                  {
                      Date = x.Key,
                      TotalThu = x.Sum(s => s.Credit),
                      TotalChi = x.Sum(s => s.Debit),
                  }).ToListAsync();

                if (dateFrom.HasValue)
                {
                    foreach (var item in res)
                    {
                        var query1 = amlObj._QueryGet(dateFrom: item.Date, state: "posted", companyId: companyId, initBal: true);
                        query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                        var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                        item.Begin = begin;
                    }

                }

            }
            else if (groupBy == "groupby:month")
            {
                res = await query.GroupBy(x => new
                {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                })
                 .Select(x => new CashBookReportItem
                 {
                     Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                     TotalThu = x.Sum(s => s.Credit),
                     TotalChi = x.Sum(s => s.Debit),
                 }).ToListAsync();


                if (dateFrom.HasValue)
                {
                    foreach (var item in res)
                    {
                        var query1 = amlObj._QueryGet(dateFrom: item.Date.Value.AbsoluteBeginOfDate(), dateTo: item.Date.Value.AbsoluteEndOfDate(), state: "posted", companyId: companyId, initBal: true);
                        query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                        var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                        item.Begin = begin;
                    }

                }

            }

            foreach (var item in res)
                item.TotalAmount = item.Begin + (item.TotalThu - item.TotalChi);

            return res;
        }

        public async Task<IEnumerable<DataInvoiceItem>> GetDataInvoices(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string resultSelection)
        {
            var amlObj = GetService<IAccountMoveLineService>();

            var types = new string[] { "cash", "bank" };
            if (resultSelection == "cash")
                types = new string[] { "cash" };
            else if (resultSelection == "bank")
                types = new string[] { "bank" };
            else if (resultSelection == "debt")
                types = new string[] { "debt" };
            else if (resultSelection == "advance")
                types = new string[] { "advance" };
            else if (resultSelection == "all")
                types = new string[] { "cash", "bank", "debt", "advance", "insurance" };

            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: companyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType == "receivable");

            var totalItems = await query.CountAsync();
            var res = await query.OrderByDescending(x => x.Date).Select(x => new DataInvoiceItem
            {
                AccountName = x.Account.Name,
                Amount = x.Credit - x.Debit,
                Date = x.Date,
                JournalName = x.Journal.Name,
                JournalType = x.Journal.Type,
                Name = x.Name,
                PartnerId = x.PartnerId.Value,
                PartnerName = x.Partner.Name,
                InvoiceOrigin = x.Move.InvoiceOrigin
            }).ToListAsync();

            return res;
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
            query = query.OrderBy(x => x.Date);

            if (val.GroupBy == "groupby:day")
            {

                res = await query.GroupBy(x => x.Date.Value.Date)
                  .Select(x => new CashBookReportItem
                  {
                      Date = x.Key,
                      TotalThu = x.Sum(s => s.Credit),
                      TotalChi = x.Sum(s => s.Debit),
                  }).ToListAsync();

                if (dateFrom.HasValue)
                {
                    foreach (var item in res)
                    {
                        var query1 = amlObj._QueryGet(dateFrom: item.Date, state: "posted", companyId: val.CompanyId, initBal: true);
                        query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                        var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                        item.Begin = begin;
                    }

                }

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
                     TotalThu = x.Sum(s => s.Credit),
                     TotalChi = x.Sum(s => s.Debit),
                 }).ToListAsync();


                if (dateFrom.HasValue)
                {
                    foreach (var item in res)
                    {
                        var query1 = amlObj._QueryGet(dateFrom: item.Date.Value.AbsoluteBeginOfDate(), dateTo: item.Date.Value.AbsoluteEndOfDate(), state: "posted", companyId: val.CompanyId, initBal: true);
                        query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType != "liquidity");
                        var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                        item.Begin = begin;
                    }

                }

            }

            foreach (var item in res)
                item.TotalAmount = item.Begin + (item.TotalThu - item.TotalChi);

            return res;
        }



    }
}
