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
            var groupSourceCount = query.GroupBy(x => x.SourceId).Select(x => new
            {
                PartnerSourceId = x.Key,
                TotalPartner = x.Count()
            }).ToList();

            var partnerSourceObj = GetService<IPartnerSourceService>();
            var sourceIds = groupSourceCount.Where(x => x.PartnerSourceId.HasValue).Select(x => x.PartnerSourceId.Value).ToList();
            var sources = await partnerSourceObj.SearchQuery(x => sourceIds.Contains(x.Id)).ToListAsync();
            var sourceDict = sources.ToDictionary(x => x.Id, x => x);

            var res = groupSourceCount.Select(x => new PartnerReportSourceOverview
            {
                PartnerSourceId = x.PartnerSourceId,
                PartnerSourceName = !x.PartnerSourceId.HasValue ? "Chưa xác định" : sourceDict[x.PartnerSourceId.Value].Name,
                TotalPartner = x.TotalPartner
            });

            return res;
        }

        public async Task<IEnumerable<PartnerGenderReportOverview>> GetPartnerReportGenderOverview(PartnerQueryableFilter val)
        {
            var partnerObj = GetService<IPartnerService>();
            var query = partnerObj.GetQueryablePartnerFilter(val);

            //group by theo độ tuổi
            var nowYear = DateTime.Now.Year;
            var ageGroupData = await query
                .Select(x => new
                {
                    DoTuoi = x.BirthYear.HasValue ? ((nowYear - x.BirthYear.Value) >= 0 && (nowYear - x.BirthYear.Value) <= 12 ? 1 : ((nowYear - x.BirthYear.Value) >= 13 && (nowYear - x.BirthYear.Value) <= 17 ? 2 : ((nowYear - x.BirthYear.Value) >= 18 && (nowYear - x.BirthYear.Value) <= 24 ? 3 : ((nowYear - x.BirthYear.Value) >= 25 && (nowYear - x.BirthYear.Value) <= 34 ? 4 : ((nowYear - x.BirthYear.Value) >= 35 && (nowYear - x.BirthYear.Value) <= 44 ? 5 : ((nowYear - x.BirthYear.Value) >= 45 && (nowYear - x.BirthYear.Value) <= 64 ? 6 : 7)))))) : 8
                })
                .GroupBy(x => x.DoTuoi)
                .Select(x => new
                {
                    DoTuoi = x.Key,
                    Count = x.Count()
                }).ToListAsync();

            //group by theo độ tuổi và giới tính
            var ageGenderGroupData = await query
             .Select(x => new
             {
                 DoTuoi = x.BirthYear.HasValue ? ((nowYear - x.BirthYear.Value) >= 0 && (nowYear - x.BirthYear.Value) <= 12 ? 1 : ((nowYear - x.BirthYear.Value) >= 13 && (nowYear - x.BirthYear.Value) <= 17 ? 2 : ((nowYear - x.BirthYear.Value) >= 18 && (nowYear - x.BirthYear.Value) <= 24 ? 3 : ((nowYear - x.BirthYear.Value) >= 25 && (nowYear - x.BirthYear.Value) <= 34 ? 4 : ((nowYear - x.BirthYear.Value) >= 35 && (nowYear - x.BirthYear.Value) <= 44 ? 5 : ((nowYear - x.BirthYear.Value) >= 45 && (nowYear - x.BirthYear.Value) <= 64 ? 6 : 7)))))) : 8,
                 Gender = x.Gender
             })
             .GroupBy(x => new { DoTuoi = x.DoTuoi, Gender = x.Gender })
             .Select(x => new
             {
                 DoTuoi = x.Key.DoTuoi,
                 Gender = x.Key.Gender,
                 Count = x.Count()
             }).ToListAsync();

            var doTuoiNameDict = new Dictionary<int, string>()
            {
                { 8, "Chưa xác định" },
                { 1, "0 - 12" },
                { 2, "13 - 17" },
                { 3, "18 - 24" },
                { 4, "25 - 34" },
                { 5, "35 - 44" },
                { 6, "45 - 64" },
                { 7, "65+" },
            };

            var res = new List<PartnerGenderReportOverview>();
            foreach (var ageGroup in ageGroupData)
            {
                var item = new PartnerGenderReportOverview()
                {
                    AgeRange = ageGroup.DoTuoi,
                    AgeRangeName = doTuoiNameDict[ageGroup.DoTuoi],
                    TotalMale = ageGenderGroupData.Where(x => x.DoTuoi == ageGroup.DoTuoi && x.Gender == "male").FirstOrDefault() != null ? ageGenderGroupData.Where(x => x.DoTuoi == ageGroup.DoTuoi && x.Gender == "male").FirstOrDefault().Count : 0,
                    TotalFemale = ageGenderGroupData.Where(x => x.DoTuoi == ageGroup.DoTuoi && x.Gender == "female").FirstOrDefault() != null ? ageGenderGroupData.Where(x => x.DoTuoi == ageGroup.DoTuoi && x.Gender == "female").FirstOrDefault().Count : 0,
                    TotalOther = ageGenderGroupData.Where(x => x.DoTuoi == ageGroup.DoTuoi && x.Gender == "other").FirstOrDefault() != null ? ageGenderGroupData.Where(x => x.DoTuoi == ageGroup.DoTuoi && x.Gender == "other").FirstOrDefault().Count : 0
                };
                res.Add(item);
            }

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

            var cityResults = await query.GroupBy(x => new { CityCode = x.CityCode, CityName = x.CityName }).Select(x => new GetPartnerForCityReportOverview
            {
                CityCode = x.Key.CityCode,
                CityName = x.Key.CityName,
                Count = x.Count()
            }).ToListAsync();

            var cityCodes = cityResults.Where(x => !x.CityCode.IsNullOrWhiteSpace()).Select(x => x.CityCode).ToList();

            var districtResults = await query.Where(x => cityCodes.Contains(x.CityCode))
                .GroupBy(x => new
                {
                    CityCode = x.CityCode,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName
                })
                .Select(x => new GetPartnerForDistrictReportOverview
                {
                    CityCode = x.Key.CityCode,
                    DistrictCode = x.Key.DistrictCode,
                    DistrictName = x.Key.DistrictName,
                    Count = x.Count()
                }).ToListAsync();

            var districtCodes = districtResults.Where(x => !x.DistrictCode.IsNullOrWhiteSpace()).Select(x => x.DistrictCode).ToList();

            var wardResult = await query.Where(x => cityCodes.Contains(x.CityCode) && districtCodes.Contains(x.DistrictCode))
                .GroupBy(x => new
                {
                    CityCode = x.CityCode,
                    DistrictCode = x.DistrictCode,
                    WardCode = x.WardCode,
                    WardName = x.WardName
                })
                .Select(x => new GetPartnerForWardReportOverview
                {
                    CityCode = x.Key.CityCode,
                    DistrictCode = x.Key.DistrictCode,
                    WardCode = x.Key.WardCode,
                    WardName = x.Key.WardName,
                    Count = x.Count()
                }).ToListAsync();

            foreach (var cityResult in cityResults)
            {
                cityResult.Districts = districtResults.Where(x => x.CityCode == cityResult.CityCode).ToList();
                foreach (var districtResult in cityResult.Districts)
                {
                    districtResult.Wards = wardResult.Where(x => x.CityCode == cityResult.CityCode && x.DistrictCode == districtResult.DistrictCode).ToList();
                }
            }

            return cityResults;
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
            var empl_dict = empObj.SearchQuery(x => x.HrJobId.HasValue && x.PartnerId.HasValue).Include(x => x.HrJob).ToDictionary(x => x.PartnerId, x => x.HrJob.Name);
            var query = amlObj._QueryGet(dateFrom: val.FromDate, dateTo: val.ToDate, initBal: true, state: "posted", companyId: val.CompanyId);
            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }
            query = query.Where(x => x.Account.Code.Equals("334"));

            var list = await query.Where(x => x.PartnerId.HasValue)
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
            var partnerObj = GetService<IPartnerService>();

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

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, companyId: val.CompanyId);
            query2 = getQueryable(query2, val);

            var groupDebitCredit = query2
                      .GroupBy(x => x.Partner.Id)
                    .Select(x => new ReportPartnerDebitRes
                    {
                        PartnerId = x.Key,
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                    });

            var list = new List<ReportPartnerDebitRes>();
            var res = new List<ReportPartnerDebitRes>();
            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, companyId: val.CompanyId);
                query = getQueryable(query, val);

                var groupBegin = query
                   .GroupBy(x => x.Partner.Id)
                   .Select(x => new ReportPartnerDebitRes
                   {
                       PartnerId = x.Key,
                       Begin = x.Sum(s => s.Debit - s.Credit),
                   });

                //To achieve full outer join in LINQ we require performing logical union of a left outer join and a right outer join result.
                //LINQ does not support full outer joins directly, the same as right outer joins

                var groupDebitCreditBeginLeft = (from dc in groupDebitCredit
                                            join b in groupBegin on dc.PartnerId equals b.PartnerId into g
                                            from sb in g.DefaultIfEmpty()
                                            select new ReportPartnerDebitRes
                                            {
                                                PartnerId = dc.PartnerId,
                                                Begin = sb == null ? 0 : sb.Begin,
                                                Debit = dc.Debit,
                                                Credit = dc.Credit
                                            }).ToList();

                var groupDebitCreditBeginRight = (from b in groupBegin
                                                 join dc in groupDebitCredit on b.PartnerId equals dc.PartnerId into g
                                                from sdc in g.DefaultIfEmpty()
                                                select new ReportPartnerDebitRes
                                                {
                                                    PartnerId = b.PartnerId,
                                                    Begin = b.Begin,
                                                    Debit = sdc == null ? 0 : sdc.Debit,
                                                    Credit = sdc == null ? 0 : sdc.Credit
                                                }).ToList();

                //paging trên groupDebitCreditBegin
                list = groupDebitCreditBeginLeft.Union(groupDebitCreditBeginRight, new ReportPartnerDebitResComparer()).ToList();
            }
            else
            {
                list = await groupDebitCredit.ToListAsync();
            }

            var partnerIds = list.Select(x => x.PartnerId).ToList();
            var partners = await partnerObj.SearchQuery(x => partnerIds.Contains(x.Id)).ToListAsync();
            var partnerDict = partners.ToDictionary(x => x.Id, x => x);

            foreach (var item in list)
            {
                if (item.Begin == 0 && item.Debit == 0 && item.Credit == 0)
                    continue;

                if (!partnerDict.ContainsKey(item.PartnerId))
                    continue;
                    
                var partner = partnerDict[item.PartnerId];

                item.PartnerName = partner.Name;
                item.PartnerDisplayName = partner.DisplayName;
                item.PartnerPhone = partner.Phone;
                item.PartnerRef = partner.Ref;
                item.End = item.Begin + item.Debit - item.Credit;
                item.DateFrom = val.FromDate;
                item.DateTo = val.ToDate;
                item.CompanyId = val.CompanyId;

                if (item.End == 0 && val.Type == "has-debt")
                    continue;

                if (item.End != 0 && val.Type == "no-debt")
                    continue;

                res.Add(item);
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
                    PartnerDisplayName = x.Partner.DisplayName
                })
                .Select(x => new
                {
                    PartnerId = x.Key.PartnerId,
                    PartnerName = x.Key.PartnerName,
                    PartnerPhone = x.Key.PartnerPhone,
                    PartnerDisplayName = x.Key.PartnerDisplayName,
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
                            PartnerDisplayName = item.PartnerDisplayName,
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
                    PartnerDisplayName = x.Partner.DisplayName,
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
                       PartnerDisplayName = x.PartnerDisplayName,
                   }, (k, g) => new
                   {
                       PartnerId = k.PartnerId,
                       PartnerName = k.PartnerName,
                       PartnerDisplayName = k.PartnerDisplayName,
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
                        PartnerDisplayName = item.PartnerDisplayName,
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
                    PartnerDisplayName = value.PartnerDisplayName,
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
                PartnerId = val.PartnerId,
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
