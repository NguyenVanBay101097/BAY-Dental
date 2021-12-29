using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
        private readonly IAsyncRepository<Partner> _partnerRepository;
        public AccountCommonPartnerReportService(IMapper mapper, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, IAsyncRepository<Partner> partnerRepository)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _partnerRepository = partnerRepository;
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

        public async Task<AccountCommonPartnerReportOverview> GetPartnerReportSumaryOverview(PartnerQueryableFilter val)
        {
            var partnerObj = GetService<IPartnerService>();
            var saleOderLineObj = GetService<ISaleOrderLineService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var query = partnerObj.GetQueryablePartnerFilter(val);

            var res = new AccountCommonPartnerReportOverview();
            res.TotalPartner = await query.CountAsync();
            res.TotalService = await saleOderLineObj.SearchQuery(x => query.Select(s => s.Id).Contains(x.OrderPartnerId.Value) && x.State == "sale" || x.State == "done").CountAsync();
            res.TotalRevenue = await saleOderLineObj.SearchQuery(x => query.Select(s => s.Id).Contains(x.OrderPartnerId.Value) && x.State == "sale" || x.State == "done").SumAsync(x => x.AmountInvoiced ?? 0);
            res.TotalRevenueExpect = await saleOderLineObj.SearchQuery(x => query.Select(s => s.Id).Contains(x.OrderPartnerId.Value) && x.State == "sale" || x.State == "done").SumAsync(x => x.PriceTotal - (x.AmountInvoiced ?? 0));
            res.TotalDebt = await amlObj.SearchQuery(x => query.Select(s => s.Id).Contains(x.PartnerId.Value) && x.Account.Code == "CNKH").SumAsync(x => x.Balance);

            return res;
        }

        public async Task<IEnumerable<PartnerReportSourceOverview>> GetPartnerReportSourceOverview(PartnerQueryableFilter val)
        {
            var partnerObj = GetService<IPartnerService>();
            var query = partnerObj.GetQueryablePartnerFilter(val);
            var items = await query.Include(x => x.Source).ToListAsync();
            var res = items.GroupBy(x => new { PartnerSourceId = x.SourceId, PartnerSourceName = x.SourceId.HasValue ? x.Source.Name : null }).Select(x => new PartnerReportSourceOverview
            {
                PartnerSourceId = x.Key.PartnerSourceId,
                PartnerSourceName = x.Key.PartnerSourceId == null ? "Chưa xác định" : x.Key.PartnerSourceName,
                TotalPartner = x.Count()
            }).ToList();

            return res;
        }

        public async Task<PartnerGenderReportOverview> GetPartnerReportGenderOverview(PartnerQueryableFilter val)
        {
            var partnerObj = GetService<IPartnerService>();
            var query = partnerObj.GetQueryablePartnerFilter(val);

            var items = await query.ToListAsync();
            var partnerGender_dict = items.GroupBy(x => x.Gender).ToDictionary(x => x.Key, x => x.ToList());
            var res = new PartnerGenderReportOverview();
            var partnerGenderItems = new List<PartnerGenderItemReportOverview>();
            var total = partnerGender_dict.Values.Sum(s => s.Count());
            var dataPartnerDate = sampleDataAgeFilter.Where(x => items.Any(s => s.BirthYear.HasValue && (!x.AgeFrom.HasValue || (DateTime.Now.Year - s.BirthYear.Value) >= x.AgeFrom.Value) && (!x.AgeTo.HasValue || (DateTime.Now.Year - s.BirthYear.Value) <= x.AgeTo.Value)) || items.Any(s => s.BirthYear == null && !x.AgeFrom.HasValue && x.AgeFrom == s.BirthYear)).ToList();
            res.XAxisChart = dataPartnerDate.Select(x => x.Name).ToList();
            res.LegendChart = partnerGender_dict.Select(x => x.Key).ToList();
            foreach (var item in partnerGender_dict)
            {
                var count = item.Value.Count();
                var percentTotal = Math.Round((double)(100 * count) / total);

                var itemValues = new List<int>();
                var itemPercentValues = new List<double>();
                foreach (var rangeAge in dataPartnerDate)
                {
                    var countPn = item.Value.AsQueryable();

                    if (rangeAge.AgeFrom.HasValue)
                        countPn = countPn.Where(x => x.BirthYear.HasValue && (DateTime.Now.Year - x.BirthYear.Value) >= rangeAge.AgeFrom.Value);

                    if (rangeAge.AgeTo.HasValue)
                        countPn = countPn.Where(x => x.BirthYear.HasValue && (DateTime.Now.Year - x.BirthYear.Value) <= rangeAge.AgeTo.Value);

                    if (!rangeAge.AgeFrom.HasValue && !rangeAge.AgeTo.HasValue)
                        countPn = countPn.Where(x => x.BirthYear == null);

                    itemValues.Add(countPn.Count());
                    var percent = countPn.Count() > 0 ? Math.Round((double)(percentTotal * countPn.Count()) / count, 2) : 0;
                    itemPercentValues.Add(percent);

                  
                }

                partnerGenderItems.Add(new PartnerGenderItemReportOverview
                {
                    PartnerGender = item.Key,
                    PartnerGenderPercent = percentTotal,
                    Count = itemValues,
                    Percent = itemPercentValues
                });
            }

            res.PartnerGenderItems = partnerGenderItems;

            return res;
        }

        public static SampleDataAgeFilter[] sampleDataAgeFilter = new SampleDataAgeFilter[] {
            new SampleDataAgeFilter {Name = "0 - 12",  AgeFrom = 0, AgeTo = 12},
            new SampleDataAgeFilter {Name = "13 - 17", AgeFrom = 13 , AgeTo = 17},
            new SampleDataAgeFilter {Name = "18 - 24", AgeFrom = 18 , AgeTo = 24},
            new SampleDataAgeFilter {Name = "25 - 34", AgeFrom = 25 , AgeTo = 34},
            new SampleDataAgeFilter {Name = "35 - 44", AgeFrom = 35 , AgeTo = 44},
            new SampleDataAgeFilter {Name = "45 - 64", AgeFrom = 45 , AgeTo = 64},
            new SampleDataAgeFilter {Name = "65+", AgeFrom = 65},
            new SampleDataAgeFilter {Name = "Không xác định" }
        };

        public async Task<IEnumerable<GetPartnerForCityReportOverview>> GetPartnerReportTreeMapOverview(PartnerQueryableFilter val)
        {
            var partnerObj = GetService<IPartnerService>();
            var query = partnerObj.GetQueryablePartnerFilter(val);

            var items = await query.ToListAsync();
            var partnerForCities = new List<GetPartnerForCityReportOverview>();
            var itemNullLocations = items.Where(x => string.IsNullOrEmpty(x.CityCode) && x.CityCode != "").ToList();
            if (itemNullLocations.Any())
            {
                partnerForCities.Add(new GetPartnerForCityReportOverview
                {
                    CityName = "Không xác định",
                    CityCode = null,
                    Count = itemNullLocations.Count(),
                    Districts = new List<GetPartnerForDistrictReportOverview>(),
                });
            }

            partnerForCities.AddRange(items.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode != "").GroupBy(x => new { CityName = x.CityName, CityCode = x.CityCode }).Select(x => new GetPartnerForCityReportOverview
            {
                CityName = !string.IsNullOrEmpty(x.Key.CityCode) ? x.Key.CityName : "Không xác định",
                CityCode = !string.IsNullOrEmpty(x.Key.CityCode) ? x.Key.CityCode : null,
                Count = x.Select(s => s.Id).Count(),
                Districts = !string.IsNullOrEmpty(x.Key.CityCode) ? x.GroupBy(x => new { DistrictName = x.DistrictName, DistrictCode = x.DistrictCode }).Select(c => new GetPartnerForDistrictReportOverview
                {
                    DistrictName = !string.IsNullOrEmpty(c.Key.DistrictCode) ? c.Key.DistrictName : "Không xác định",
                    DistrictCode = !string.IsNullOrEmpty(c.Key.DistrictCode) ? c.Key.DistrictCode : null,
                    Count = c.Select(e => e.Id).Count(),
                    Wards = c.GroupBy(x => new { WardName = x.WardName, WardCode = x.WardCode }).Select(e => new GetPartnerForWardReportOverview
                    {
                        WardName = !string.IsNullOrEmpty(e.Key.WardCode) ? e.Key.WardName : "Không xác định",
                        WardCode = e.Key.WardCode,
                        Count = e.Select(z => z.Id).Count()
                    }).ToList(),
                }).ToList() : new List<GetPartnerForDistrictReportOverview>(),
            }).ToList());

            var res = partnerForCities;
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
            var empObj = GetService<IEmployeeService>();
            var empl_dict = await empObj.SearchQuery(x => x.HrJobId.HasValue && x.PartnerId.HasValue).Include(x => x.HrJob).Include(x => x.User).ThenInclude(x => x.Partner).Distinct().ToDictionaryAsync(x => x.PartnerId, x => x.HrJob.Name);
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
                   JobName = empl_dict.ContainsKey(x.Key.PartnerId) ? empl_dict[x.Key.PartnerId] : null,
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
                        HrJobName = item.JobName,
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
                        JobName = empl_dict.ContainsKey(x.Key.PartnerId) ? empl_dict[x.Key.PartnerId] : null,
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
                        HrJobName = item.JobName,
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
                    HrJobName = value.HrJobName
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
                   }).ToListAsync();
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

            var dict = new Dictionary<Guid, ReportPartnerAdvance>();
            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: val.CompanyId, initBal: true);

                if (!string.IsNullOrWhiteSpace(val.Search))
                {
                    query1 = query1.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
                }

                var list1 = await query1.Where(x => x.Account.Code == "KHTU" || x.Account.Code == "HTU").GroupBy(x => new
                {
                    PartnerId = x.Partner.Id,
                    PartnerName = x.Partner.Name,
                    PartnerPhone = x.Partner.Phone,
                })
                .Select(x => new
                {
                    PartnerId = x.Key.PartnerId,
                    PartnerName = x.Key.PartnerName,
                    PartnerPhone = x.Key.PartnerPhone,
                    Balance = x.Sum(s => s.Debit - s.Credit),
                }).ToListAsync();

                foreach (var item in list1)
                {
                    if (!dict.ContainsKey(item.PartnerId))
                    {
                        dict.Add(item.PartnerId, new ReportPartnerAdvance()
                        {
                            PartnerId = item.PartnerId,
                            PartnerName = item.PartnerName,
                            PartnerPhone = item.PartnerPhone,
                            Begin = -item.Balance
                        });
                    }
                }
            }

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => x.Account.Code == "KHTU" || x.Account.Code == "HTU");

            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }

            var list2 = await query
                .Select(x => new
                {
                    PartnerId = x.Partner.Id,
                    PartnerName = x.Partner.Name,
                    PartnerPhone = x.Partner.Phone,
                    Debit = x.Debit,
                    Credit = x.Credit,
                    AccountCode = x.Account.Code
                })
                   .GroupBy(x => new
                   {
                       PartnerId = x.PartnerId,
                       PartnerName = x.PartnerName,
                       PartnerPhone = x.PartnerPhone,
                   }, (k, g) => new
                   {
                       PartnerId = k.PartnerId,
                       PartnerName = k.PartnerName,
                       PartnerPhone = k.PartnerPhone,
                       Debit = g.Sum(s => s.AccountCode == "KHTU" ? s.Debit : 0),
                       Credit = g.Sum(s => s.AccountCode == "KHTU" ? s.Credit : 0),
                       Return = g.Sum(s => s.AccountCode == "HTU" ? s.Debit : 0)
                   })
                   .ToListAsync();

            foreach (var item in list2)
            {
                if (!dict.ContainsKey(item.PartnerId))
                {
                    dict.Add(item.PartnerId, new ReportPartnerAdvance()
                    {
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerPhone = item.PartnerPhone,
                        Debit = item.Credit,
                        Credit = item.Debit,
                        Refund = item.Return,
                    });

                    continue;
                }

                dict[item.PartnerId].Debit = item.Credit;
                dict[item.PartnerId].Credit = item.Debit;
                dict[item.PartnerId].Refund = item.Return;
            }

            var res = new List<ReportPartnerAdvance>();
            foreach (var item in dict)
            {
                var begin = dict[item.Key].Begin;
                var debit = dict[item.Key].Debit;
                var credit = dict[item.Key].Credit;
                var retrn = dict[item.Key].Refund;
                if (begin == 0 && debit == 0 && credit == 0 && retrn == 0)
                    continue;

                var end = begin + debit - credit - retrn;
                var value = item.Value;
                res.Add(new ReportPartnerAdvance
                {
                    Begin = begin,
                    Debit = debit,
                    Credit = credit,
                    Refund = retrn,
                    End = end,
                    PartnerName = value.PartnerName,
                    PartnerPhone = value.PartnerPhone,
                    PartnerId = item.Key,
                    DateFrom = val.DateFrom,
                    DateTo = val.DateTo,
                    CompanyId = val.CompanyId
                });
            }

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

            query = query.Where(x => x.Account.Code == "KHTU" || x.Account.Code == "HTU");

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            var res = await query.Select(x => new ReportPartnerAdvanceDetail
            {
                Date = x.Date,
                Debit = x.Credit,
                Credit = x.Debit,
                InvoiceOrigin = x.Move.InvoiceOrigin,
                Name = x.Name,
                PartnerId = x.PartnerId
            }).ToListAsync();

            decimal begin = 0;
            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: val.CompanyId, initBal: true);
                query1 = query1.Where(x => x.Account.Code == "KHTU" || x.Account.Code == "HTU");
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

        public async Task<ReportPartnerAdvancePrintVM> ReportPartnerAdvancePrint(ReportPartnerAdvanceFilter val)
        {
            var data = await ReportPartnerAdvance(val);
            var res = new ReportPartnerAdvancePrintVM();
            var companyObj = GetService<ICompanyService>();
            res.DateFrom = val.DateFrom;
            res.DateTo = val.DateTo;
            res.Company = val.CompanyId.HasValue ? _mapper.Map<CompanyPrintVM>(await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync()) : null;
            res.User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId));
            res.ReportPartnerAdvances = data;

            return res;
        }
    }

}
