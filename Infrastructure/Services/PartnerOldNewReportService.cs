using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PartnerOldNewReportService : IPartnerOldNewReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public PartnerOldNewReportService(CatalogDbContext context, IMapper mapper, IUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
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
                      TotalNewPartner = x.GroupBy(s => s.PartnerId).Select(x => x.LastOrDefault()).Where(x => x.Type == "KHM").Count(),
                      TotalOldPartner = x.GroupBy(s => s.PartnerId).Select(x => x.LastOrDefault()).Where(x => x.Type == "KHC").Count(),

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
        public IQueryable<SaleOrder> SumReportQuery(PartnerOldNewReportReq val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var orderSearchQr = saleOrderObj.SearchQuery(x => x.State != "draft"); // cần xem lại làm state cho tổng quát

            var query = orderSearchQr;// là searchquery theo filter
            var orderCompanyQr = orderSearchQr;// searchquery theo chi nhánh;

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateOrder.Date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateOrder.Date <= val.DateTo.Value.AbsoluteEndOfDate());
            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId);
                orderCompanyQr = orderCompanyQr.Where(x => x.CompanyId == val.CompanyId);

            }
            //filter partner
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);
            else
            {
                if (!string.IsNullOrEmpty(val.CityCode))
                    query = query.Where(x => x.Partner.CityCode == val.CityCode);
                if (!string.IsNullOrEmpty(val.DistrictCode))
                    query = query.Where(x => x.Partner.DistrictCode == val.DistrictCode);
                if (!string.IsNullOrEmpty(val.WardCode))
                    query = query.Where(x => x.Partner.WardCode == val.WardCode);
                //if (val.SourceId.HasValue)
                //    query = query.Where(x => x.Partner.SourceId == val.SourceId);
                if (val.SourceIds.Any())
                    query = query.Where(x => x.Partner.SourceId.HasValue && val.SourceIds.Contains(x.Partner.SourceId.Value));

                if (val.IsHasNullSourceId)
                    query = query.Where(x => !x.Partner.SourceId.HasValue);
                if (!string.IsNullOrEmpty(val.Gender))
                    query = query.Where(x => x.Partner.Gender == val.Gender);

                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search)
                                           || x.Partner.Ref.Contains(val.Search) || x.Partner.Phone.Contains(val.Search));
                if (val.CategIds.Any())
                {
                    var partnerCategoryRelService = GetService<IPartnerPartnerCategoryRelService>();
                    var filterPartnerQr =
                           from pcr in partnerCategoryRelService.SearchQuery(x => val.CategIds.Contains(x.CategoryId))
                           select new
                           {
                               PartnerId = pcr.PartnerId
                           };
                    query = query.Join(filterPartnerQr, o => o.PartnerId, pc => pc.PartnerId, (o, pc) => o);
                }

            }

            if (!string.IsNullOrEmpty(val.TypeReport))
            {
                var querySub = query.Select(x => new
                {
                    Id = x.Id,
                    PartnerId = x.PartnerId,
                    IsNew = !orderCompanyQr.Any(z => z.PartnerId == x.PartnerId && z.DateOrder < x.DateOrder)
                });

                if (val.TypeReport == "new")
                    querySub = querySub.Where(x => x.IsNew);
                if (val.TypeReport == "old")
                    querySub = querySub.Where(x => !x.IsNew);
                query = query.Join(querySub, o => o.Id, s => s.Id, (o, s) => o);
            }

            //var resQr = query.GroupBy(x => x.PartnerId);
            //if (val.TypeReport == "old")
            //    resQr = resQr.Where(x => orderBaseQr.Where(h => h.PartnerId == x.Key).Count() > 1);
            //if (val.TypeReport == "new" && val.DateFrom.HasValue) 
            //    resQr = resQr.Where(x => orderBaseQr.Where(h => h.PartnerId == x.Key && h.DateOrder < val.DateFrom.Value).Count() == 0);
            return query;
        }

        public async Task<decimal> SumReVenue(PartnerOldNewReportReq val)
        {
            var query = SumReportQuery(val);
            var res = query.GroupBy(x => x.PartnerId).Select(x => new { Key = x.Key, TotalPaid = x.Sum(z => z.TotalPaid), LastDateOfTreatment = x.Max(z => z.DateOrder) });
            //.SumAsync(x => x.TotalPaid ?? 0);
            if (!string.IsNullOrEmpty(val.OverInterval) && val.OverIntervalNbr.HasValue)
            {
                if (val.OverInterval == "month")
                    res = res.Where(x => x.LastDateOfTreatment.AddMonths(val.OverIntervalNbr.Value) < DateTime.Now);
            }

            return await res.SumAsync(x => x.TotalPaid ?? 0);
        }

        public async Task<int> SumReport(PartnerOldNewReportReq val)
        {
            var query = SumReportQuery(val);
            var res = query.GroupBy(x => x.PartnerId).Select(x => new { Key = x.Key, LastDateOfTreatment = x.Max(z => z.DateOrder) });

            if (!string.IsNullOrEmpty(val.OverInterval) && val.OverIntervalNbr.HasValue)
            {
                if (val.OverInterval == "month")
                    res = res.Where(x => x.LastDateOfTreatment.AddMonths(val.OverIntervalNbr.Value) < DateTime.Now);
            }
            return await res.CountAsync();
        }

        public async Task<IEnumerable<PartnerOldNewReportByWard>> ReportByWard(PartnerOldNewReportByWardReq val)
        {

            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.PartnerOldNewInSaleOrders.Where(x => company_ids.Contains(x.CompanyId) && x.SaleOrder.State != "draft");
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (!string.IsNullOrEmpty(val.CityCode))
                query = query.Where(x => x.Partner.CityCode == val.CityCode);
            else
                query = query.Where(x => string.IsNullOrEmpty(x.Partner.CityCode));

            if (!string.IsNullOrEmpty(val.DistrictCode))
                query = query.Where(x => x.Partner.DistrictCode == val.DistrictCode);
            else
                query = query.Where(x => string.IsNullOrEmpty(x.Partner.DistrictCode));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateOrder >= val.DateFrom.Value.AbsoluteBeginOfDate());

            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateOrder <= val.DateTo.Value.AbsoluteEndOfDate());

            //group by theo wardcode, wardname, những dòng isNew true là số khách mới, những dòng isnew false là số khách quay lại, count distince
            var result = await query.GroupBy(x => new
            {
                WardCode = x.Partner.WardCode,
                WardName = x.Partner.WardName,
                PartnerId = x.PartnerId,
                IsNew = x.IsNew
            }).Select(x => new
            {
                WardCode = x.Key.WardCode,
                WardName = x.Key.WardName,
                IsNew = x.Key.IsNew,
                //PartnerCount = x.Sum(s => 1),
                PartnerRevenue = x.Sum(s => s.TotalPaid),
            })
            .GroupBy(x => new
            {
                WardCode = x.WardCode,
                WardName = x.WardName,
            })
            .Select(x => new PartnerOldNewReportByWard()
            {
                WardCode = x.Key.WardCode,
                WardName = x.Key.WardName,
                PartnerNewCount = x.Sum(s => s.IsNew ? 1 : 0),
                PartnerNewRevenue = x.Sum(s => s.IsNew ? s.PartnerRevenue : 0),
                PartnerOldCount = x.Sum(s => !s.IsNew ? 1 : 0),
                PartnerOldRevenue = x.Sum(s => !s.IsNew ? s.PartnerRevenue : 0),
            })
            .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<PartnerOldNewReportByIsNewItem>> ReportByIsNew(PartnerOldNewReportByIsNewReq val)
        {

            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.PartnerOldNewInSaleOrders.Where(x => company_ids.Contains(x.CompanyId) && x.SaleOrder.State != "draft");
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateOrder >= val.DateFrom.Value.AbsoluteBeginOfDate());

            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateOrder <= val.DateTo.Value.AbsoluteEndOfDate());

            var result = await query.GroupBy(x => new
            {
                PartnerId = x.PartnerId,
                IsNew = x.IsNew,
            }).Select(x => new
            {
                IsNew = x.Key.IsNew,
                PartnerId = x.Key.PartnerId,
                TotalOrder = x.Sum(s => 1)
            })
            .GroupBy(x => x.IsNew)
            .Select(x => new PartnerOldNewReportByIsNewItem()
            {
                IsNew = x.Key ? 1 : 0,
                PartnerTotal = x.Sum(s => 1),
            })
            .ToListAsync();

            return result;
        }

        public IQueryable<PartnerInfoTemplate> GetReportQuery(PartnerOldNewReportReq val)
        {
            var saleOrderObj = GetService<ISaleOrderService>();
            var irProperyObj = GetService<IIRPropertyService>();
            var partnerObj = GetService<IPartnerService>();
            var accInvreportObj = GetService<IAccountInvoiceReportService>();
            var companyId = CompanyId;


            var pnOderQr = SumReportQuery(val).GroupBy(x => x.PartnerId)
                .Select(x => new { PartnerId = x.Key, Sum = x.Sum(z => z.TotalPaid ?? 0), LastDateOfTreatment = x.Max(z => z.DateOrder) });

            var pnQr = partnerObj.SearchQuery(x => x.Customer);

            var partnerOrderStateQr = from v in saleOrderObj.SearchQuery(x => !val.CompanyId.HasValue || x.CompanyId == val.CompanyId)
                                      group v by v.PartnerId into g
                                      select new
                                      {
                                          PartnerId = g.Key,
                                          CountSale = g.Sum(x => x.State == "sale" ? 1 : 0),
                                          CountDone = g.Sum(x => x.State == "done" ? 1 : 0)
                                      };

            var resQr = from pnOrder in pnOderQr
                        join pn in pnQr on pnOrder.PartnerId equals pn.Id
                        from pos in partnerOrderStateQr.Where(x => x.PartnerId == pnOrder.PartnerId).DefaultIfEmpty()
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
                            Revenue = pnOrder.Sum,
                            SourceName = pn.Source.Name,
                            LastDateOfTreatment = pnOrder.LastDateOfTreatment
                        };

            return resQr;
        }

        public async Task<PagedResult2<PartnerOldNewReportRes>> GetReport(PartnerOldNewReportReq val)
        {
            var memberLevelObj = GetService<IMemberLevelService>();
            var cateObj = GetService<IPartnerCategoryService>();
            var partnerCategoryRelObj = GetService<IPartnerPartnerCategoryRelService>();

            var ResponseQr = GetReportQuery(val);
            if (!string.IsNullOrEmpty(val.OverInterval) && val.OverIntervalNbr.HasValue)
            {
                if (val.OverInterval == "month")
                    ResponseQr = ResponseQr.Where(x => x.LastDateOfTreatment.AddMonths(val.OverIntervalNbr.Value) < DateTime.Now);
            }
            var count = await ResponseQr.CountAsync();
            if (val.Limit > 0)
                ResponseQr = ResponseQr.Skip(val.Offset).Take(val.Limit);
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
            return new PagedResult2<PartnerOldNewReportRes>(count, val.Offset, val.Limit)
            {
                Items = items
            };

        }

        public async Task<PartnerOldNewReportPrint> GetReportPrint(PartnerOldNewReportReq val)
        {
            val.Limit = 0;
            var data = await GetReport(val);
            var res = new PartnerOldNewReportPrint()
            {
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                Data = data.Items,
                User = _mapper.Map<ApplicationUserSimple>(await _userService.GetCurrentUser())
            };

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                var company = await companyObj.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
                res.Company = _mapper.Map<CompanyPrintVM>(company);
            }
            return res;
        }

        public async Task<PagedResult2<SaleOrderBasic>> GetSaleOrderPaged(GetSaleOrderPagedReq val)
        {
            var valParam = _mapper.Map<PartnerOldNewReportReq>(val);
            var query = SumReportQuery(valParam);

            var count = await query.CountAsync();
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);
            var items = _mapper.Map<IEnumerable<SaleOrderBasic>>(await query.ToListAsync());
            return new PagedResult2<SaleOrderBasic>(count, val.Offset, val.Limit)
            {
                Items = items
            };

        }

        public async Task<PartnerOldNewReportExcel> GetReportExcel(PartnerOldNewReportReq val)
        {
            val.Limit = 0;
            var data = await GetReport(val);
            var res = new PartnerOldNewReportExcel()
            {
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                Data = _mapper.Map<IEnumerable<PartnerOldNewReportResExcel>>(data.Items)
            };

            var valParam = _mapper.Map<GetSaleOrderPagedReq>(val);
            var allLines = await GetSaleOrderPaged(valParam);
            foreach (var item in res.Data)
            {
                item.Lines = allLines.Items.Where(x => x.PartnerId == item.Id).ToList();
            }
            return res;
        }
    }
}
