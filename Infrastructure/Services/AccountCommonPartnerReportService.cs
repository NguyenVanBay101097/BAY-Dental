﻿using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountCommonPartnerReportService : IAccountCommonPartnerReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountCommonPartnerReportService(IMapper mapper, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IEnumerable<AccountCommonPartnerReportItem>> ReportSummary(AccountCommonPartnerReportSearch val)
        {
            var today = DateTime.Today;
            var date_from = val.FromDate;
            var date_to = val.ToDate;
            if (date_to.HasValue)
                date_to = date_to.Value.AbsoluteEndOfDate();

            var amlObj = (IAccountMoveLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountMoveLineService));
            string[] accountTypes = null;
            if (val.ResultSelection == "customer")
                accountTypes = new string[] { "receivable" };
            else if (val.ResultSelection == "supplier")
                accountTypes = new string[] { "payable" };

            var dict = new Dictionary<Guid, AccountCommonPartnerReportItem>();
            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, state: "posted", companyId: val.CompanyId);
                query = query.Where(x => accountTypes.Contains(x.Account.InternalType) && x.PartnerId.HasValue &&
                (!val.PartnerId.HasValue || x.PartnerId == val.PartnerId));

                if (!string.IsNullOrWhiteSpace(val.Search))
                {
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
                }

                var list = await query
                   .GroupBy(x => new
                   {
                       PartnerId = x.Partner.Id,
                       PartnerName = x.Partner.Name,
                       PartnerRef = x.Partner.Ref,
                       PartnerPhone = x.Partner.Phone,
                       Type = x.Account.InternalType
                   })
                   .Select(x => new
                   {
                       PartnerId = x.Key.PartnerId,
                       PartnerName = x.Key.PartnerName,
                       PartnerRef = x.Key.PartnerRef,
                       PartnerPhone = x.Key.PartnerPhone,
                       x.Key.Type,
                       InitialBalance = x.Sum(s => s.Debit - s.Credit),
                   }).ToListAsync();

                foreach (var item in list)
                {
                    if (!dict.ContainsKey(item.PartnerId))
                    {
                        dict.Add(item.PartnerId, new AccountCommonPartnerReportItem()
                        {
                            PartnerId = item.PartnerId,
                            PartnerName = item.PartnerName,
                            PartnerRef = item.PartnerRef,
                            PartnerPhone = item.PartnerPhone,
                            ResultSelection = val.ResultSelection,
                            DateFrom = date_from,
                            DateTo = date_to
                        });
                    }

                    if (item.Type == "receivable")
                        dict[item.PartnerId].Begin = item.InitialBalance;
                    else if (item.Type == "payable")
                        dict[item.PartnerId].Begin = -item.InitialBalance;
                }
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: val.CompanyId);
            query2 = query2.Where(x => accountTypes.Contains(x.Account.InternalType) && x.PartnerId.HasValue &&
             (!val.PartnerId.HasValue || x.PartnerId == val.PartnerId));

            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query2 = query2.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }

            var list2 = await query2
                      .GroupBy(x => new
                      {
                          PartnerId = x.Partner.Id,
                          PartnerName = x.Partner.Name,
                          PartnerRef = x.Partner.Ref,
                          PartnerPhone = x.Partner.Phone,
                          Type = x.Account.InternalType
                      })
                    .Select(x => new
                    {
                        PartnerId = x.Key.PartnerId,
                        PartnerName = x.Key.PartnerName,
                        PartnerRef = x.Key.PartnerRef,
                        PartnerPhone = x.Key.PartnerPhone,
                        x.Key.Type,
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                    }).ToListAsync();

            foreach (var item in list2)
            {
                if (!dict.ContainsKey(item.PartnerId))
                {
                    dict.Add(item.PartnerId, new AccountCommonPartnerReportItem()
                    {
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerRef = item.PartnerRef,
                        PartnerPhone = item.PartnerPhone,
                        ResultSelection = val.ResultSelection,
                        DateFrom = date_from,
                        DateTo = date_to
                    });
                }

                if (item.Type == "receivable")
                {
                    dict[item.PartnerId].Debit = item.Debit;
                    dict[item.PartnerId].Credit = item.Credit;
                }
                else if (item.Type == "payable")
                {
                    dict[item.PartnerId].Debit = item.Credit;
                    dict[item.PartnerId].Credit = item.Debit;
                }
            }

            var res = new List<AccountCommonPartnerReportItem>();
            foreach (var item in dict)
            {
                var begin = dict[item.Key].Begin;
                var debit = dict[item.Key].Debit;
                var credit = dict[item.Key].Credit;
                var end = begin + debit - credit;
                if ((val.Display == "not_zero" && end == 0) || (begin == 0 && debit == 0 && credit == 0))
                    continue;
                var value = item.Value;
                res.Add(new AccountCommonPartnerReportItem
                {
                    PartnerId = item.Key,
                    DateFrom = date_from,
                    DateTo = date_to,
                    ResultSelection = val.ResultSelection,
                    PartnerRef = value.PartnerRef,
                    Begin = begin,
                    Debit = debit,
                    Credit = credit,
                    End = end,
                    PartnerName = value.PartnerName,
                    PartnerPhone = value.PartnerPhone,
                });
            }
            return res;
        }
        public async Task<AccountCommonPartnerReportPrint> ReportSummaryPrint(AccountCommonPartnerReportSearch val)
        {
            var data = await ReportSummary(val);
            var res = new AccountCommonPartnerReportPrint()
            {
                DateFrom = val.FromDate,
                DateTo = val.ToDate,
                Data = data,
            };
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                res.Company = _mapper.Map<CompanyPrintVM>(await companyObj.SearchQuery(x => x.Id == val.CompanyId)
                    .Include(x => x.Partner).FirstOrDefaultAsync());
            }
            return res;
        }

        public async Task<IEnumerable<AccountCommonPartnerReportItemDetail>> ReportDetail(AccountCommonPartnerReportItem val)
        {
            var date_from = val.DateFrom;
            var date_to = val.DateTo;
            var amlObj = (IAccountMoveLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountMoveLineService));

            string[] accountTypes = null;
            if (val.ResultSelection == "customer")
                accountTypes = new string[] { "receivable" };
            else if (val.ResultSelection == "supplier")
                accountTypes = new string[] { "payable" };
            var sign = val.ResultSelection == "supplier" ? -1 : 1;
            decimal begin = 0;
            var res = new List<AccountCommonPartnerReportItemDetail>();

            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, state: "posted");
                query = query.Where(x => accountTypes.Contains(x.Account.InternalType) && x.PartnerId == val.PartnerId);
                begin = (await query.SumAsync(x => x.Debit - x.Credit)) * sign;
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted");
            query2 = query2.Where(x => accountTypes.Contains(x.Account.InternalType) && x.PartnerId == val.PartnerId);
            var list2 = query2.OrderBy(x => x.DateCreated)
                    .Select(x => new AccountCommonPartnerReportItemDetail
                    {
                        Date = x.Date,
                        MoveName = x.Move.Name,
                        Name = x.Name,
                        Ref = x.Move.Ref,
                        Debit = x.Account.InternalType == "payable" ? x.Credit : x.Debit,
                        Credit = x.Account.InternalType == "payable" ? x.Debit : x.Credit,
                    }).ToList();


            foreach (var item in list2)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Debit - item.Credit;
                begin = item.End;
            }

            return list2;
        }

        public IQueryable<AccountMoveLine> _GetPartnerReportQuery(AccountCommonPartnerReportSearchV2 data)
        {
            string[] accountTypes = new string[] { "receivable", "payable" };
            if (data.ResultSelection == "customer")
                accountTypes = new string[] { "receivable" };
            else if (data.ResultSelection == "supplier")
                accountTypes = new string[] { "payable" };

            if (data.FromDate.HasValue)
                data.FromDate = data.FromDate.Value.AbsoluteBeginOfDate();
            if (data.ToDate.HasValue)
                data.ToDate = data.ToDate.Value.AbsoluteEndOfDate();

            var date_from = data.FromDate;
            var date_to = data.ToDate;

            var amlObj = (IAccountMoveLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountMoveLineService));
            var query = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: data.CompanyId);
            query = query.Where(x => accountTypes.Contains(x.Account.InternalType));
            if (data.PartnerIds.Any())
                query = query.Where(x => x.PartnerId.HasValue && data.PartnerIds.Contains(x.PartnerId.Value));
            return query;
        }

        public async Task<AccountCommonPartnerReportSearchV2Result> ReportSumaryPartner(AccountCommonPartnerReportSearchV2 val)
        {
            var query = _GetPartnerReportQuery(val);
            var data = await query.GroupBy(x => 0).Select(x => new AccountCommonPartnerReportSearchV2Result
            {
                Debit = x.Sum(s => s.Debit),
                Credit = x.Sum(s => s.Credit),
                InitialBalance = x.Sum(s => s.Debit) - x.Sum(s => s.Credit)
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<IEnumerable<AccountCommonPartnerReportItem>> ReportSalaryEmployee(AccountCommonPartnerReportSearch val)
        {
            val.ResultSelection = "employee";
            var today = DateTime.Today;
            var date_from = val.FromDate.HasValue ? val.FromDate.Value : new DateTime(today.Year, today.Month, 1);
            var date_to = val.ToDate.HasValue ? val.ToDate.Value.AddDays(1).AddMinutes(-1) : today.AddDays(1).AddMinutes(-1); //23h59
            var dict = new Dictionary<Guid, AccountCommonPartnerReportItem>();
            var amlObj = (IAccountMoveLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountMoveLineService));
            var query = amlObj._QueryGet(dateFrom: val.FromDate, dateTo: val.ToDate, initBal: true, state: "posted", companyId: val.CompanyId);
            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }
            query = query.Where(x => x.Account.Code.Equals("334"));

            var list = await query
               .GroupBy(x => new
               {
                   PartnerId = x.Partner.Id,
                   PartnerName = x.Partner.Name,
                   PartnerRef = x.Partner.Ref,
                   PartnerPhone = x.Partner.Phone,
                   Type = x.Account.InternalType
               })
               .Select(x => new
               {
                   PartnerId = x.Key.PartnerId,
                   PartnerName = x.Key.PartnerName,
                   PartnerRef = x.Key.PartnerRef,
                   PartnerPhone = x.Key.PartnerPhone,
                   x.Key.Type,
                   InitialBalance = x.Sum(s => s.Debit - s.Credit),
               }).ToListAsync();

            foreach (var item in list)
            {
                if (!dict.ContainsKey(item.PartnerId))
                {
                    dict.Add(item.PartnerId, new AccountCommonPartnerReportItem()
                    {
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerRef = item.PartnerRef,
                        PartnerPhone = item.PartnerPhone,
                        ResultSelection = val.ResultSelection,
                        DateFrom = date_from,

                        DateTo = date_to
                    });
                }
                dict[item.PartnerId].Begin = -item.InitialBalance;
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: val.CompanyId);
            query2 = query2.Where(x => x.Account.Code.Equals("334"));

            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query2 = query2.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }

            var list2 = await query2
                      .GroupBy(x => new
                      {
                          PartnerId = x.Partner.Id,
                          PartnerName = x.Partner.Name,
                          PartnerRef = x.Partner.Ref,
                          PartnerPhone = x.Partner.Phone,
                          Type = x.Account.InternalType
                      })
                    .Select(x => new
                    {
                        PartnerId = x.Key.PartnerId,
                        PartnerName = x.Key.PartnerName,
                        PartnerRef = x.Key.PartnerRef,
                        PartnerPhone = x.Key.PartnerPhone,
                        x.Key.Type,
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                    }).ToListAsync();

            foreach (var item in list2)
            {
                if (!dict.ContainsKey(item.PartnerId))
                {
                    dict.Add(item.PartnerId, new AccountCommonPartnerReportItem()
                    {
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerRef = item.PartnerRef,
                        PartnerPhone = item.PartnerPhone,
                        ResultSelection = val.ResultSelection,
                        DateFrom = date_from,
                        DateTo = date_to
                    });
                }

                dict[item.PartnerId].Debit = item.Credit;
                dict[item.PartnerId].Credit = item.Debit;
            }

            var res = new List<AccountCommonPartnerReportItem>();
            foreach (var item in dict)
            {
                var begin = dict[item.Key].Begin;
                var debit = dict[item.Key].Debit;
                var credit = dict[item.Key].Credit;
                var end = begin + debit - credit;
                var value = item.Value;
                res.Add(new AccountCommonPartnerReportItem
                {
                    PartnerId = item.Key,
                    DateFrom = date_from,
                    DateTo = date_to,
                    ResultSelection = val.ResultSelection,
                    PartnerRef = value.PartnerRef,
                    Begin = begin,
                    Debit = debit,
                    CompanyId = value.CompanyId,
                    Credit = credit,
                    End = end,
                    PartnerName = value.PartnerName,
                    PartnerPhone = value.PartnerPhone,
                });
            }
            return res;
        }

        public async Task<IEnumerable<AccountCommonPartnerReportItemDetail>> ReportSalaryEmployeeDetail(AccountCommonPartnerReportItem val)
        {
            var date_from = val.DateFrom;
            var date_to = val.DateTo;
            var amlObj = (IAccountMoveLineService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountMoveLineService));

            var sign = -1;
            decimal begin = 0;
            var res = new List<AccountCommonPartnerReportItemDetail>();

            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, state: "posted", companyId: val.CompanyId);
                query = query.Where(x => x.Account.Code.Equals("334") && x.PartnerId == val.PartnerId);
                begin = (await query.SumAsync(x => x.Debit - x.Credit)) * sign;
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: val.CompanyId);
            query2 = query2.Where(x => x.Account.Code.Equals("334") && x.PartnerId == val.PartnerId);
            var list2 = query2.OrderBy(x => x.DateCreated)
                    .Select(x => new AccountCommonPartnerReportItemDetail
                    {
                        Date = x.Date,
                        MoveName = x.Move.Name,
                        Name = x.Name,
                        Ref = x.Move.Ref,
                        Debit = x.Credit,
                        Credit = x.Debit
                    }).ToList();


            foreach (var item in list2)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Debit - item.Credit;
                begin = item.End;
            }

            return list2;
        }

        public async Task<IEnumerable<AccountMoveBasic>> GetListReportPartner(AccountCommonPartnerReportSearch val)
        {
            var accMoveObj = (IAccountMoveService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IAccountMoveService));
            var query = accMoveObj.SearchQuery(x => x.AmountResidual != 0 && x.Type == "in_invoice" && x.InvoicePaymentState == "not_paid");
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.InvoiceOrigin.Contains(val.Search));

            var results = await query.ToListAsync();
            return _mapper.Map<IEnumerable<AccountMoveBasic>>(results);
        }
        public T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }
        public async Task<IEnumerable<ReportPartnerDebitRes>> ReportPartnerDebit(ReportPartnerDebitReq val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();

            IQueryable<AccountMoveLine> getQueryable(IQueryable<AccountMoveLine> query, ReportPartnerDebitReq val)
            {
                query = query.Where(x => x.Account.Code == "CNKH");
                if (val.PartnerId.HasValue)
                    query = query.Where(x => x.PartnerId == val.PartnerId);

                if (!string.IsNullOrWhiteSpace(val.Search))
                {
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
                }
                return query;
            }

            var today = DateTime.Today;
            var date_from = val.FromDate.HasValue ? val.FromDate.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var date_to = val.ToDate.HasValue ? val.ToDate.Value.AbsoluteEndOfDate() : (DateTime?)null;

            var res = new List<ReportPartnerDebitRes>();
            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, companyId: val.CompanyId);
                query = getQueryable(query, val);

                res = await query
                   .GroupBy(x => new
                   {
                       PartnerId = x.Partner.Id,
                       PartnerName = x.Partner.Name,
                       PartnerRef = x.Partner.Ref,
                       PartnerPhone = x.Partner.Phone,
                   })
                   .Select(x => new ReportPartnerDebitRes
                   {
                       PartnerId = x.Key.PartnerId,
                       PartnerName = x.Key.PartnerName,
                       PartnerRef = x.Key.PartnerRef,
                       PartnerPhone = x.Key.PartnerPhone,
                       Begin = x.Sum(s => s.Debit - s.Credit),
                   }).Where(x => x.Begin != 0).ToListAsync();
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, companyId: val.CompanyId);
            query2 = getQueryable(query2, val);

            var list2 = await query2
                      .GroupBy(x => new
                      {
                          PartnerId = x.Partner.Id,
                          PartnerName = x.Partner.Name,
                          PartnerRef = x.Partner.Ref,
                          PartnerPhone = x.Partner.Phone,
                      })
                    .Select(x => new ReportPartnerDebitRes
                    {
                        PartnerId = x.Key.PartnerId,
                        PartnerName = x.Key.PartnerName,
                        PartnerRef = x.Key.PartnerRef,
                        PartnerPhone = x.Key.PartnerPhone,
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                        Begin = 0
                    }).ToListAsync();

            foreach (var item in list2)
            {
                var resItem = res.FirstOrDefault(x => x.PartnerId == item.PartnerId);
                if (resItem == null)
                {
                    res.Add(item);
                }
                else
                {
                    resItem.Debit = item.Debit;
                    resItem.Credit = item.Credit;
                }
            }

            foreach (var item in res)
            {
                item.End = item.Begin + item.Debit - item.Credit;
                item.DateFrom = val.FromDate;
                item.DateTo = val.ToDate;
                item.CompanyId = val.CompanyId;
            }

            return res;
        }

        public async Task<IEnumerable<ReportPartnerDebitDetailRes>> ReportPartnerDebitDetail(ReportPartnerDebitDetailReq val)
        {
            var today = DateTime.Today;
            var date_from = val.FromDate.HasValue ? val.FromDate.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var date_to = val.ToDate.HasValue ? val.ToDate.Value.AbsoluteEndOfDate() : (DateTime?)null;

            var amlObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();

            decimal begin = 0;
            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, companyId: val.CompanyId);
                query = query.Where(x => x.Account.Code == "CNKH");
                if (val.PartnerId.HasValue)
                    query = query.Where(x => x.PartnerId == val.PartnerId);
                begin = await query.SumAsync(x => x.Debit - x.Credit);
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, companyId: val.CompanyId);
            query2 = query2.Where(x => x.Account.Code == "CNKH");
            if (val.PartnerId.HasValue)
                query2 = query2.Where(x => x.PartnerId == val.PartnerId);

            var list2 = query2.OrderBy(x => x.DateCreated)
                    .Select(x => new ReportPartnerDebitDetailRes
                    {
                        Date = x.Date,
                        Debit = x.Debit,
                        Credit = x.Credit,
                        InvoiceOrigin = x.Move.InvoiceOrigin,
                        Ref = x.Ref,
                        PartnerId = x.PartnerId.Value
                    }).ToList();


            foreach (var item in list2)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Debit - item.Credit;
                begin = item.End;
            }

            return list2;

        }

        public async Task<IEnumerable<ReportPartnerAdvance>> ReportPartnerAdvance(ReportPartnerAdvanceFilter val)
        {
            var amlObj = GetService<IAccountMoveLineService>();

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);

            query = query.Where(x => x.Account.Code == "KHTU");

            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }

            var res = new List<ReportPartnerAdvance>();
            res = await query
                   .GroupBy(x => new
                   {
                       PartnerId = x.Partner.Id,
                       PartnerName = x.Partner.Name,
                       PartnerPhone = x.Partner.Phone,
                   })
                   .Select(x => new ReportPartnerAdvance
                   {
                       PartnerId = x.Key.PartnerId,
                       PartnerName = x.Key.PartnerName,
                       PartnerPhone = x.Key.PartnerPhone,
                       Debit = x.Sum(s => s.Credit),
                       Credit = x.Sum(s => s.Debit)
                   }).ToListAsync();

            if (dateFrom.HasValue)
            {
                foreach (var item in res)
                {
                    var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: val.CompanyId, initBal: true);
                    query1 = query1.Where(x => x.Account.Code == "KHTU" );
                    query1 = query1.Where(x => x.PartnerId == item.PartnerId);
                    var begin = await query1.SumAsync(x => x.Credit - x.Debit);
                    item.Begin = begin;
                }
            }

            foreach (var item in res)
                item.End = item.Begin + (item.Debit - item.Credit);

            return res;
        }

        public async Task<IEnumerable<ReportPartnerAdvanceDetail>> ReportPartnerAdvanceDetail(ReportPartnerAdvanceDetailFilter val)
        {
            var amlObj = GetService<IAccountMoveLineService>();

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);

            query = query.Where(x => x.Account.Code == "KHTU");

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            var res = await query.Select(x => new ReportPartnerAdvanceDetail
            {
                Date = x.Date,
                Debit = x.Credit,
                Credit = x.Debit,
                InvoiceOrigin = x.Move.InvoiceOrigin,
                Ref = x.Ref,             
            }).ToListAsync();

            decimal begin = 0;
            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: val.CompanyId, initBal: true);
                query1 = query1.Where(x => x.Account.Code == "KHTU");
                if (val.PartnerId.HasValue)
                    query1 = query1.Where(x => x.PartnerId == val.PartnerId);
                begin = await query1.SumAsync(x => x.Credit - x.Debit);
            }

            foreach (var item in res)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Debit - item.Credit;
                begin = item.End;
            }

            return res;

        }

        public async Task<ReportPartnerDebitSummaryRes> ReportPartnerDebitSummary(ReportPartnerDebitReq val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();

            var date_from = val.FromDate.HasValue ? val.FromDate.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var date_to = val.ToDate.HasValue ? val.ToDate.Value.AbsoluteEndOfDate() : (DateTime?)null;
            var query = amlObj._QueryGet(dateTo: date_to, dateFrom: date_from, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => x.Account.Code == "CNKH");
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            var result = await query.GroupBy(x => 0).Select(x => new ReportPartnerDebitSummaryRes
            {
                Debit = x.Sum(s => s.Debit),
                Credit = x.Sum(s => s.Credit),
                Balance = x.Sum(s => s.Debit - s.Credit)
            }).ToListAsync();

            return new ReportPartnerDebitSummaryRes
            {
                Debit = result.Count > 0 ? result[0].Debit : 0,
                Credit = result.Count > 0 ? result[0].Credit : 0,
                Balance = result.Count > 0 ? result[0].Balance : 0,
            };
        }

        public async Task<ReportPartnerDebitPrintVM> PrintReportPartnerDebit(ReportPartnerDebitReq val)
        {
            var companyObj = GetService<ICompanyService>();
            var result = new ReportPartnerDebitPrintVM();
            var reportPartnerDebitDetailReq = new ReportPartnerDebitDetailReq()
            {
                FromDate = val.FromDate,
                ToDate = val.ToDate,
                CompanyId = val.CompanyId,
                PartnerId = val.PartnerId
            };
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            result.User = _mapper.Map<ApplicationUserSimple>(user);
            if (val.CompanyId.HasValue)
            {
                result.Company = _mapper.Map<CompanyPrintVM>(await companyObj.SearchQuery(x => x.Id == val.CompanyId)
                    .Include(x => x.Partner).FirstOrDefaultAsync());
            }
            var lines = _mapper.Map<IEnumerable<ReportPartnerDebitPrint>>(await ReportPartnerDebit(val));
            foreach (var line in lines)
            {

                reportPartnerDebitDetailReq.PartnerId = line.PartnerId;
                line.Lines = await ReportPartnerDebitDetail(reportPartnerDebitDetailReq);
            }
            result.ReportPartnerDebitLines = lines;
            result.DateFrom = val.FromDate;
            result.DateTo = val.ToDate;

            return result;

        }     

        public async Task<IEnumerable<ReportPartnerDebitExcel>> ExportReportPartnerDebitExcel(ReportPartnerDebitReq val)
        {
            var companyObj = GetService<ICompanyService>();
            var reportPartnerDebitDetailReq = new ReportPartnerDebitDetailReq()
            {
                FromDate = val.FromDate,
                ToDate = val.ToDate,
                CompanyId = val.CompanyId,
                PartnerId = val.PartnerId
            };
            var data = _mapper.Map<IEnumerable<ReportPartnerDebitExcel>>(await ReportPartnerDebit(val));
            foreach (var line in data)
            {

                reportPartnerDebitDetailReq.PartnerId = line.PartnerId;
                line.Lines = await ReportPartnerDebitDetail(reportPartnerDebitDetailReq);
            }

            return data;

        }

        private string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }
    }

}
