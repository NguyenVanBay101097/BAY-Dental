using ApplicationCore.Entities;
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

        public IQueryable<PartnerReportOverviewItem> GetQueryablePartnerReportOverview(AccountCommonPartnerReportOverviewFilter val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrderLineObj = GetService<ISaleOrderLineService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var accObj = GetService<IAccountAccountService>();
            var irProperyObj = GetService<IIRPropertyService>();
            var cardCardObj = GetService<ICardCardService>();
            var serviceCardCardObj = GetService<IServiceCardCardService>();
            var partnerObj = GetService<IPartnerService>();
            var partnerSourceObj = GetService<IPartnerSourceService>();

            var mainQuery = partnerObj.SearchQuery(x => x.Active && x.Customer);

            if (val.CompanyId.HasValue)
                mainQuery = mainQuery.Where(x => x.CompanyId == val.CompanyId);

            if (val.PartnerCompanyId.HasValue)
                mainQuery = mainQuery.Where(x => x.CompanyId == val.PartnerCompanyId);

            if (!string.IsNullOrEmpty(val.CityCode))
                mainQuery = mainQuery.Where(x => x.CityCode == val.CityCode);

            if (!string.IsNullOrEmpty(val.DistrictCode))
                mainQuery = mainQuery.Where(x => x.DistrictCode == val.DistrictCode);

            if (!string.IsNullOrEmpty(val.WardCode))
                mainQuery = mainQuery.Where(x => x.WardCode == val.WardCode);

            if (val.CategIds.Any())
            {
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();

                var filterPartnerQr = from pcr in partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId))
                                      group pcr by pcr.PartnerId into g
                                      select g.Key;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            if (val.CardTypeIds.Any())
            {
                var filterPartnerQr =
                   from pcr in cardCardObj.SearchQuery(x => val.CardTypeIds.Contains(x.TypeId))
                   select pcr.PartnerId;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            if (val.PartnerSourceIds.Any())
            {
                var filterPartnerQr =
                 from pcr in partnerObj.SearchQuery(x => val.PartnerSourceIds.Contains(x.SourceId.Value))
                 select pcr.Id;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            if (val.AgeFrom.HasValue)
            {
                var filterPartnerQr =
              from pcr in partnerObj.SearchQuery(x => x.BirthYear.HasValue && val.AgeFrom >= int.Parse(x.GetAge))
              select pcr.Id;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            if (val.AgeTo.HasValue)
            {
                var filterPartnerQr =
              from pcr in partnerObj.SearchQuery(x => x.BirthYear.HasValue && val.AgeTo <= int.Parse(x.GetAge))
              select pcr.Id;

                mainQuery = from a in mainQuery
                            join pbk in filterPartnerQr on a.Id equals pbk
                            select a;
            }

            var partnerOrderStateQr = from v in saleOrderObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                      group v by v.PartnerId into g
                                      select new
                                      {
                                          PartnerId = g.Key,
                                          CountSale = g.Sum(x => x.State == "sale" ? 1 : 0),
                                          CountDone = g.Sum(x => x.State == "done" ? 1 : 0)
                                      };

            var PartnerResidualQr = (from s in saleOrderLineObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                     where s.State == "sale" || s.State == "done"
                                     group s by s.OrderPartnerId into g
                                     select new
                                     {
                                         PartnerId = g.Key,
                                         TotalService = g.Count(),
                                         OrderResidual = g.Sum(x => x.PriceTotal - x.AmountInvoiced),
                                         TotalPaid = g.Sum(x => x.AmountInvoiced)
                                     });



            var partnerDebtQr = from aml in amlObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                join acc in accObj.SearchQuery()
                                on aml.AccountId equals acc.Id
                                where acc.Code == "CNKH"
                                group aml by aml.PartnerId into g
                                select new
                                {
                                    PartnerId = g.Key,
                                    TotalDebit = g.Sum(x => x.Balance)
                                };

            var cardCardQr = from card in cardCardObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                             select card;






            var ResponseQr = from p in mainQuery
                             from pr in PartnerResidualQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from pd in partnerDebtQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from pos in partnerOrderStateQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             from card in cardCardQr.Where(x => x.PartnerId == p.Id).DefaultIfEmpty()
                             select new PartnerReportOverviewItem
                             {
                                 PartnerId = p.Id,
                                 PartnerGender = p.Gender,
                                 PartnerAge = !string.IsNullOrEmpty(p.GetAge) ? p.GetAge : null,
                                 PartnerSourceId = p.SourceId.HasValue ? p.SourceId : null,
                                 PartnerSourceName = p.SourceId.HasValue ? p.Source.Name : null,
                                 OrderState = pos.CountSale > 0 ? "sale" : (pos.CountDone > 0 ? "done" : "draft"),
                                 TotalService = pr.TotalService,
                                 TotalRevenue = pr.TotalPaid ?? 0,
                                 TotalRevenueExpect = pr.OrderResidual ?? 0,
                                 TotalDebt = pd.TotalDebit
                             };

            if (val.RevenueFrom.HasValue)
            {
                ResponseQr = ResponseQr.Where(x => x.TotalRevenue >= val.RevenueFrom);
            }

            if (val.RevenueTo.HasValue)
            {
                ResponseQr = ResponseQr.Where(x => x.TotalRevenue <= val.RevenueTo);
            }

            if (val.RevenueExpectFrom.HasValue)
            {
                ResponseQr = ResponseQr.Where(x => x.TotalRevenueExpect >= val.RevenueFrom);
            }

            if (val.RevenueExpectTo.HasValue)
            {
                ResponseQr = ResponseQr.Where(x => x.TotalRevenueExpect <= val.RevenueTo);
            }

            if (val.IsRevenueExpect.HasValue)
            {
                ResponseQr = ResponseQr.Where(x => val.IsRevenueExpect.Value == true ? x.TotalRevenueExpect > 0 : x.TotalRevenueExpect == 0);
            }


            if (val.IsDebt.HasValue)
            {
                ResponseQr = ResponseQr.Where(x => val.IsDebt.Value == true ? x.TotalDebt > 0 : x.TotalDebt == 0);
            }

            if (!string.IsNullOrEmpty(val.OrderState))
            {
                ResponseQr = ResponseQr.Where(x => x.OrderState == val.OrderState);
            }

            return ResponseQr;
        }

        public async Task<AccountCommonPartnerReportOverview> GetPartnerReportSumaryOverview(AccountCommonPartnerReportOverviewFilter val)
        {
            var query = GetQueryablePartnerReportOverview(val);
            var res = new AccountCommonPartnerReportOverview();
            res.TotalPartner = await query.CountAsync();
            res.TotalService = await query.SumAsync(x => x.TotalService);
            res.TotalRevenue = await query.SumAsync(x => x.TotalRevenue);
            res.TotalRevenueExpect = await query.SumAsync(x => x.TotalRevenueExpect);
            res.TotalDebt = await query.SumAsync(x => x.TotalDebt);

            return res;
        }

        public async Task<IEnumerable<PartnerReportSourceOverview>> GetPartnerReportSourceOverview(AccountCommonPartnerReportOverviewFilter val)
        {
            var query = GetQueryablePartnerReportOverview(val);
            var items = await query.ToListAsync();
            var res = items.GroupBy(x => new { PartnerSourceId = x.PartnerSourceId, PartnerSourceName = x.PartnerSourceName }).Select(x => new PartnerReportSourceOverview
            {
                PartnerSourceId = x.Key.PartnerSourceId,
                PartnerSourceName = x.Key.PartnerSourceId == null ? "Chưa xác định" : x.Key.PartnerSourceName,
                TotalPartner = x.Count()
            }).ToList();

            return res;
        }

        public async Task<PartnerGenderReportOverview> GetPartnerReportGenderOverview(AccountCommonPartnerReportOverviewFilter val)
        {
            var sampleDataAge = sampleDataAgeFilter;
            var query = GetQueryablePartnerReportOverview(val);

            var items = await query.ToListAsync();
            var partnerGender_dict = items.GroupBy(x => x.PartnerGender).ToDictionary(x => x.Key, x => x.ToList());
            var res = new PartnerGenderReportOverview();
            var partnerGenderItems = new List<PartnerGenderItemReportOverview>();
            var total = partnerGender_dict.Values.Sum(s => s.Count());
            res.LegendChart = sampleDataAgeFilter.Select(x => x.Name).ToList();

            foreach (var item in partnerGender_dict)
            {
                var count = item.Value.Count();
                var percentTotal = (int)Math.Round((double)(100 * count) / total);

                var itemValues = new List<int>();
                var itemPercentValues = new List<int>();
                foreach (var rangeAge in sampleDataAge)
                {
                    var countPn = item.Value.AsQueryable();

                    if (rangeAge.AgeFrom.HasValue)
                        countPn = countPn.Where(x => !string.IsNullOrEmpty(x.PartnerAge) && int.Parse(x.PartnerAge) >= rangeAge.AgeFrom.Value);

                    if (rangeAge.AgeTo.HasValue)
                        countPn = countPn.Where(x => !string.IsNullOrEmpty(x.PartnerAge) && int.Parse(x.PartnerAge) <= rangeAge.AgeTo.Value);

                    if (!rangeAge.AgeFrom.HasValue && !rangeAge.AgeTo.HasValue)
                        countPn = countPn.Where(x => x.PartnerAge == null);

                    itemValues.Add(countPn.Count());
                    var percent = countPn.Count() > 0 ? (int)Math.Round((double)(percentTotal * countPn.Count()) / count) : 0;
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
