using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PartnerOldNewReportService : IPartnerOldNewReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PartnerOldNewReportService(CatalogDbContext context, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<PartnerOldNewReport> _GetQueryAble(PartnerOldNewReportSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.PartnerOldNewReports.Where(x => company_ids.Contains(x.CompanyId));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);
            return query;
        }

        public async Task<PartnerOldNewReportVM> GetSumaryPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var query = _GetQueryAble(val);
            var partners = query.AsEnumerable().GroupBy(x => x.PartnerId).Select(x => new
            {
                PartnerId = x.Key,
                LastDateOrder = x.Max(s => s.Date),
                PartnerOldNew = x.LastOrDefault(),
            }).ToList();

            var result = new PartnerOldNewReportVM()
            {
                TotalNewPartner = partners.Where(x => x.PartnerOldNew.Type == "KHM").Count(),
                TotalOldPartner = partners.Where(x => x.PartnerOldNew.Type == "KHC").Count(),
            };

            return result;
        }

        public async Task<IEnumerable<PartnerOldNewReportVM>> GetPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var query = _GetQueryAble(val);

            var result = query.AsEnumerable().GroupBy(x => new
            {
                Year = x.Date.Year,
                WeekOfYear = GetIso8601WeekOfYear(x.Date)
            })
                  .Select(x => new PartnerOldNewReportVM
                  {
                      WeekOfYear = x.Key.WeekOfYear,
                      Year = x.Key.Year,
                      OrderLines = _mapper.Map<IEnumerable<PartnerOldNewReportVMDetail>>(x.GroupBy(s => s.PartnerId).Select(x => x.LastOrDefault()).ToList()),
                      TotalNewPartner = x.GroupBy(s => s.PartnerId).Select(x => x.LastOrDefault()).Where(x=>x.Type =="KHM").Count(),
                      TotalOldPartner = x.GroupBy(s => s.PartnerId).Select(x => x.LastOrDefault()).Where(x=>x.Type =="KHC").Count(),

                  }).ToList();
            return result;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        private T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }
        public IQueryable<IGrouping<Guid, SaleOrder>> SumReportQuery(PartnerOldNewReportSumReq val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var pnOrderSearchQuery = saleOrderObj.SearchQuery(x=> x.State != "draft");

            var query = pnOrderSearchQuery;
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateOrder.Date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateOrder.Date <= val.DateTo.Value.AbsoluteBeginOfDate());
            if(val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var pnOrderQrBase = query.GroupBy(x => x.PartnerId);
            if (val.TypeReport == "old")
                pnOrderQrBase = pnOrderQrBase.Where(x => pnOrderSearchQuery.Where(h => h.PartnerId == x.Key).Count() > 1);
            if (val.TypeReport == "new" && val.DateFrom.HasValue)
                pnOrderQrBase = pnOrderQrBase.Where(x => pnOrderSearchQuery.Where(h => h.PartnerId == x.Key && h.DateOrder.Date < val.DateFrom.Value.Date).Count() == 0);
            return pnOrderQrBase;
        }
        public async Task<int> SumReport(PartnerOldNewReportSumReq val)
        {
            var query = SumReportQuery(val);
            return await query.Select(x=> x.Key).CountAsync();
        }
        public IQueryable<PartnerInfoTemplate> GetReportQuery(PartnerOldNewReportReq val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var irProperyObj = GetService<IIRPropertyService>();
            var partnerObj = GetService<IPartnerService>();
            var accInvreportObj = GetService<IAccountInvoiceReportService>();
            var companyId = CompanyId;

            var pnOrderQrBase = SumReportQuery(new PartnerOldNewReportSumReq(val.DateFrom, val.DateTo, val.CompanyId, val.TypeReport)) ;

            var pnOderQr = from v in pnOrderQrBase
                           select new
                           {
                               PartnerId = v.Key,
                           };

            var pnQr = partnerObj.SearchQuery(x => x.Customer);
            if (!string.IsNullOrEmpty(val.CityCode))
                pnQr = pnQr.Where(x=> x.CityCode == val.CityCode);
            if (!string.IsNullOrEmpty(val.DistrictCode))
                pnQr = pnQr.Where(x => x.DistrictCode == val.DistrictCode);
            if (!string.IsNullOrEmpty(val.WardCode))
                pnQr = pnQr.Where(x => x.WardCode == val.WardCode);
            if (val.SourceId.HasValue)
                pnQr = pnQr.Where(x => x.SourceId == x.SourceId);
            if (!string.IsNullOrEmpty(val.Gender))
                pnQr = pnQr.Where(x => x.Gender == val.Gender);
            if (!string.IsNullOrEmpty(val.Search))
                pnQr = pnQr.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search)
                                       || x.Ref.Contains(val.Search) || x.Phone.Contains(val.Search));
            if (val.CategIds.Any())
            {
                var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();
                var filterPartnerQr =
                       from pcr in partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId))
                       select new
                       {
                           PartnerId = pcr.PartnerId
                       };
                pnQr = pnQr.Where(x => filterPartnerQr.Any(i => i.PartnerId == x.Id));
            }

            var partnerOrderStateQr = from v in saleOrderObj.SearchQuery()
                                      group v by v.PartnerId into g
                                      select new
                                      {
                                          PartnerId = g.Key,
                                          CountSale = g.Sum(x => x.State == "sale" ? 1 : 0),
                                          CountDone = g.Sum(x => x.State == "done" ? 1 : 0)
                                      };

            var irPropertyQr = from ir in irProperyObj.SearchQuery(x => x.Name == "member_level" && x.Field.Model == "res.partner")
                               select ir;

            var RevenueQr = from v in accInvreportObj.GetRevenueReportQuery(new RevenueReportQueryCommon(null, null, companyId: val.CompanyId))
                            group v by v.PartnerId into g
                            select new
                            {
                                PartnerId = g.Key.Value,
                                PriceSubTotal = Math.Abs(g.Sum(z => z.PriceSubTotal))
                            };

            var resQr = from pnOrder in pnOderQr
                        //from pn in pnQr.Where(x => x.Id == pnOrder.PartnerId).DefaultIfEmpty()
                        join pn in pnQr on pnOrder.PartnerId equals pn.Id 
                        from pos in partnerOrderStateQr.Where(x => x.PartnerId == pnOrder.PartnerId).DefaultIfEmpty()
                        from ir in irPropertyQr.Where(x => !string.IsNullOrEmpty(x.ResId) && x.ResId.Contains(pnOrder.PartnerId.ToString().ToLower())).DefaultIfEmpty()
                        from pnRevenue in RevenueQr.Where(x => x.PartnerId == pnOrder.PartnerId).DefaultIfEmpty()
                        select new PartnerInfoTemplate
                        {
                            Id = pn.Id,
                            Name = pn.Name,
                            NameNoSign = pn.NameNoSign,
                            Ref = pn.Ref,
                            Phone = pn.Phone,
                            BirthYear = pn.BirthYear,
                            DisplayName = pn.DisplayName,
                            CityName = pn.CityName,
                            DistrictName = pn.DistrictName,
                            WardName = pn.WardName,
                            Street = pn.Street,
                            Gender = pn.Gender,
                            OrderState = pos.CountSale > 0 ? "sale" : (pos.CountDone > 0 ? "done" : "draft"),
                            MemberLevelId = ir.ValueReference,
                            Revenue = pnRevenue.PriceSubTotal,
                            SourceName = pn.Source.Name
                        };

            if (val.MemberLevelId.HasValue)
            {
                resQr = resQr.Where(x => x.MemberLevelId.Contains(val.MemberLevelId.Value.ToString()));
            }

            return resQr;
        }

        public async Task<IEnumerable<PartnerOldNewReportRes>> GetReport(PartnerOldNewReportReq val)
        {
            var memberLevelObj = GetService<IMemberLevelService>();
            var cateObj = GetService<IPartnerCategoryService>();
            var partnerCategoryRelObj = GetService<IPartnerPartnerCategoryRelService>();

            var ResponseQr = GetReportQuery(val);
            var count = await ResponseQr.CountAsync();
            var res = await ResponseQr.ToListAsync();

            var cateList = await partnerCategoryRelObj.SearchQuery(x => res.Select(i => i.Id).Contains(x.PartnerId)).Include(x => x.Category).ToListAsync();
            var categDict = cateList.GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.Select(s => s.Category));
            var memberLevelIds = res.Where(x => !string.IsNullOrEmpty(x.MemberLevelId)).Select(x => new Guid(x.MemberLevelId.Substring(x.MemberLevelId.IndexOf(",") + 1))).Distinct().ToList();
            var memberLevels = await memberLevelObj.SearchQuery(x => memberLevelIds.Contains(x.Id)).ToListAsync();
            var memberLevelDict = memberLevels.ToDictionary(x => x.Id, x => x);
            foreach (var item in res)
            {
                if (!string.IsNullOrEmpty(item.MemberLevelId))
                {
                    var pnMemberLevelId = new Guid(item.MemberLevelId.Substring(item.MemberLevelId.IndexOf(",") + 1));
                    if (memberLevelDict.ContainsKey(pnMemberLevelId))
                    {
                        var level = memberLevelDict[pnMemberLevelId];
                        item.MemberLevel = _mapper.Map<MemberLevelBasic>(level);
                    }
                }
                item.Categories = _mapper.Map<List<PartnerCategoryBasic>>(categDict.ContainsKey(item.Id) ? categDict[item.Id] : new List<PartnerCategory>());
            }
            var items = _mapper.Map<IEnumerable<PartnerOldNewReportRes>>(res);
            return items;

        }
    }
}
