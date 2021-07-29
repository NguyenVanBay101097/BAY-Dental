using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SaleReportService : ISaleReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaleReportService(CatalogDbContext context, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public async Task<IEnumerable<SaleReportTopServicesCs>> GetTopServices(SaleReportTopServicesFilter val)
        {
            var companyId = CompanyId;
            var query = _context.SaleReports.AsQueryable();
            query = query.Where(x => x.CompanyId == companyId);
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId.Equals(val.PartnerId));
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId.Equals(val.CompanyId));
            if (val.CategId.HasValue)
                query = query.Where(x => x.CategId.Equals(val.CategId));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State.ToLower()));
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo);


            if (val.ByInvoice)
            {
                return await query.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                }).Select(x => new SaleReportTopServicesCs
                {
                    ProductId = x.Key.ProductId ?? Guid.Empty,
                    PriceTotalSum = x.Sum(y => y.PriceTotal),
                    ProductUOMQtySum = x.Sum(y => y.ProductUOMQty),
                    ProductName = x.Key.ProductName
                }).OrderByDescending(x => x.PriceTotalSum).ThenByDescending(x => x.ProductUOMQtySum).Take(val.Number)
                            .ToListAsync();
            }
            else
            {
                return await query.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                }).Select(x => new SaleReportTopServicesCs
                {
                    ProductId = x.Key.ProductId ?? Guid.Empty,
                    ProductUOMQtySum = x.Sum(y => y.ProductUOMQty),
                    PriceTotalSum = x.Sum(y => y.PriceTotal),
                    ProductName = x.Key.ProductName
                }).OrderByDescending(x => x.ProductUOMQtySum).ThenByDescending(x => x.PriceTotalSum).Take(val.Number)
                            .ToListAsync();
            }
        }

        public async Task<IEnumerable<SaleReportItem>> GetReport(SaleReportSearch val)
        {
            //thời gian, tiền thực thu, doanh thu, còn nợ
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();

            var query = _context.SaleReports.Where(x => !x.CompanyId.HasValue || company_ids.Contains(x.CompanyId.Value));
            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= dateFrom);
            }

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                query = query.Where(x => states.Contains(x.State));
            }

            if (val.IsQuotation.HasValue)
            {
                if (val.IsQuotation.Value)
                    query = query.Where(x => x.IsQuotation == val.IsQuotation);
                else
                    query = query.Where(x => !x.IsQuotation.HasValue || x.IsQuotation == val.IsQuotation);
            }

            if (val.GroupBy == "customer")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));

                var result = await query.GroupBy(x => new
                {
                    PartnerId = x.PartnerId,
                    PartnerName = x.Partner.Name
                })
                  .Select(x => new SaleReportItem
                  {
                      PartnerId = x.Key.PartnerId,
                      Name = x.Key.PartnerName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy,
                      IsQuotation = val.IsQuotation,
                      CompanyId = val.CompanyId
                  }).ToListAsync();
                return result;
            }
            if (val.GroupBy == "employee")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Employee.Name.Contains(val.Search));

                var result = await query.GroupBy(x => new
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.Name
                })
                  .Select(x => new SaleReportItem
                  {
                      EmployeeId = x.Key.EmployeeId,
                      Name = x.Key.EmployeeName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy,
                      IsQuotation = val.IsQuotation,
                      CompanyId = val.CompanyId
                  }).ToListAsync();
                return result;
            }
            if (val.GroupBy == "product")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search) ||
                    x.Product.DefaultCode.Contains(val.Search));

                var result = await query.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                })
                  .Select(x => new SaleReportItem
                  {
                      ProductId = x.Key.ProductId,
                      Name = x.Key.ProductName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy,
                      IsQuotation = val.IsQuotation,
                      CompanyId = val.CompanyId
                  }).ToListAsync();
                return result;
            }
            if (val.GroupBy == "date:quarter")
            {
                var result = await query.GroupBy(x => new
                {
                    x.Date.Year,
                    QuarterOfYear = (x.Date.Month - 1) / 3,
                })
                  .Select(x => new SaleReportItem
                  {
                      Year = x.Key.Year,
                      QuarterOfYear = x.Key.QuarterOfYear,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy,
                      IsQuotation = val.IsQuotation,
                      CompanyId = val.CompanyId
                  }).ToListAsync();
                foreach (var item in result)
                {
                    item.Date = new DateTime(item.Year.Value, item.QuarterOfYear.Value * 3 + 1, 1);
                    item.Name = $"Quý {item.QuarterOfYear}, {item.Year}";
                }
                return result;
            }
            else if (val.GroupBy == "date:month" || val.GroupBy == "date")
            {
                var result = await query.GroupBy(x => new
                {
                    x.Date.Year,
                    x.Date.Month,
                })
                  .Select(x => new SaleReportItem
                  {
                      Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy,
                      IsQuotation = val.IsQuotation,
                      CompanyId = val.CompanyId
                  }).ToListAsync();
                foreach (var item in result)
                {
                    item.Name = item.Date.Value.ToString("MM/yyyy");
                }
                return result;
            }
            if (val.GroupBy == "date:week")
            {
                var result = query.AsEnumerable().GroupBy(x => new
                {
                    Year = x.Date.Year,
                    WeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        x.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                })
                  .Select(x => new SaleReportItem
                  {
                      Year = x.Key.Year,
                      WeekOfYear = x.Key.WeekOfYear,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy,
                      IsQuotation = val.IsQuotation,
                      CompanyId = val.CompanyId
                  }).ToList();
                foreach (var item in result)
                {
                    item.Date = DateUtils.FirstDateOfWeekISO8601(item.Year.Value, item.WeekOfYear.Value);
                    item.Name = $"Tuần {item.WeekOfYear}, {item.Year}";
                }
                return result;
            }
            else if (val.GroupBy == "date:day")
            {
                var result = await query.GroupBy(x => new
                {
                    x.Date.Year,
                    x.Date.Month,
                    x.Date.Day,
                })
                   .Select(x => new SaleReportItem
                   {
                       Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                       ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                       PriceTotal = x.Sum(s => s.PriceTotal),
                       GroupBy = val.GroupBy,
                       IsQuotation = val.IsQuotation,
                       CompanyId = val.CompanyId
                   }).ToListAsync();
                foreach (var item in result)
                {
                    item.Name = item.Date.Value.ToString("dd/MM/yyyy");
                }
                return result;
            }
            else
            {
                var result = await query.GroupBy(x => 0)
                  .Select(x => new SaleReportItem
                  {
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                  }).ToListAsync();
                return result;
            }
        }

        public async Task<PagedResult2<SaleOrderLineDisplay>> GetReportService(SaleReportSearch val)
        {
            var lineObj = GetService<ISaleOrderLineService>();
            var query = lineObj.SearchQuery(x => (!x.Order.IsQuotation.HasValue || x.Order.IsQuotation == false))
                .Include(x => x.Order)
                .Include(x => x.Employee)
                .Include(x => x.OrderPartner)
                .Include(x => x.Product)
                .Include("SaleOrderLineToothRels.Tooth")
                .AsQueryable();

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Order.DateOrder >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Order.DateOrder <= val.DateTo.Value);

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => !x.Order.State.Equals(val.State));

            var items = await query.ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<SaleOrderLineDisplay>(totalItems, 0, 0)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderLineDisplay>>(items)
            };
        }

        public async Task<IEnumerable<SaleReportItemDetail>> GetReportDetail(SaleReportItem val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();

            var query = _context.SaleReports.Where(x => !x.CompanyId.HasValue || company_ids.Contains(x.CompanyId.Value));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

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

            if (!string.IsNullOrEmpty(val.State))
            {
                var states = val.State.Split(",");
                query = query.Where(x => states.Contains(x.State));
            }

            if (val.IsQuotation.HasValue)
            {
                if (val.IsQuotation.Value)
                    query = query.Where(x => x.IsQuotation == val.IsQuotation);
                else
                    query = query.Where(x => !x.IsQuotation.HasValue || x.IsQuotation == val.IsQuotation);
            }

            if (val.GroupBy == "customer")
                query = query.Where(x => x.PartnerId == val.PartnerId);
            else if (val.GroupBy == "employee")
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            else if (val.GroupBy == "product")
                query = query.Where(x => x.ProductId == val.ProductId);
            else if (val.GroupBy == "date" || val.GroupBy == "date:month")
            {
                var dateFrom = val.Date.Value.AbsoluteBeginOfDate();
                var dateTo = dateFrom.AddMonths(1);
                query = query.Where(x => x.Date >= dateFrom && x.Date < dateTo);
            }
            else if (val.GroupBy == "date:quarter")
            {
                var dateFrom = val.Date.Value.AbsoluteBeginOfDate();
                var dateTo = dateFrom.AddMonths(3);
                query = query.Where(x => x.Date >= dateFrom && x.Date < dateTo);
            }
            else if (val.GroupBy == "date:week")
            {
                var dateFrom = val.Date.Value.AbsoluteBeginOfDate();
                var dateTo = dateFrom.AddDays(7);
                query = query.Where(x => x.Date >= dateFrom && x.Date < dateTo);
            }
            else if (val.GroupBy == "date:day")
            {
                var dateFrom = val.Date.Value.AbsoluteBeginOfDate();
                var dateTo = dateFrom.AddDays(1);
                query = query.Where(x => x.Date >= dateFrom && x.Date < dateTo);
            }

            var result = await query.Select(x => new SaleReportItemDetail
            {
                Date = x.Date,
                Name = x.Name,
                PriceTotal = x.PriceTotal,
                ProductName = x.Product.Name,
                ProductUOMQty = x.ProductUOMQty
            }).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SaleReportPartnerItem>> GetReportPartner(SaleReportPartnerSearch val)
        {
            //Thông kê tình hình điều trị của khách hàng, tính số phiếu điều trị, lần cuối điều trị, để từ đó lọc những khách hàng mới hay cũ
            var saleObj = GetService<ISaleOrderService>();
            var companyId = CompanyId;

            var states = new string[] { "draft", "cancel" };
            var query = saleObj.SearchQuery(x => x.CompanyId == companyId && !states.Contains(x.State) && (!x.IsQuotation.HasValue || x.IsQuotation == false));
            var data = await query.GroupBy(x => x.PartnerId).Select(x => new SaleReportPartnerItem
            {
                PartnerId = x.Key,
                OrderCount = x.Count(),
                LastDateOrder = x.Max(s => s.DateOrder)
            }).ToListAsync();

            var partner_ids = data.Select(x => x.PartnerId).ToList();
            var partnerObj = GetService<IPartnerService>();
            var partners = await partnerObj.GetList(partner_ids);
            var partnerDict = partners.ToDictionary(x => x.Id, x => x);

            var result = new List<SaleReportPartnerItem>();
            foreach (var item in data)
            {
                if (val.PartnerDisplay == "new" && item.OrderCount != 1)
                    continue;
                if (val.PartnerDisplay == "old" && item.OrderCount == 1)
                    continue;
                var partner = partnerDict[item.PartnerId];
                item.PartnerName = partner.Name;
                item.PartnerPhone = partner.Phone;
                item.PartnerDisplayName = partner.DisplayName;

                result.Add(item);
            }

            return result;
        }

        public async Task<IEnumerable<SaleReportPartnerItem>> GetReportPartnerV2(SaleReportPartnerSearch val)
        {
            var saleObj = GetService<ISaleOrderService>();
            var companyId = CompanyId;

            var states = new string[] { "draft", "cancel" };
            var query = saleObj.SearchQuery(x => x.CompanyId == companyId && !states.Contains(x.State) && (!x.IsQuotation.HasValue || x.IsQuotation == false));
            var data = await query.GroupBy(x => x.PartnerId).Select(x => new SaleReportPartnerItem
            {
                PartnerId = x.Key,
                OrderCount = x.Count(),
                LastDateOrder = x.Max(s => s.DateOrder)
            }).ToListAsync();

            var partner_ids = data.Select(x => x.PartnerId).ToList();
            var partnerObj = GetService<IPartnerService>();
            var partnerDict = await partnerObj.SearchQuery(x => x.Customer == true).ToDictionaryAsync(x => x.Id, x => x);
            var partnersLast = await partnerObj.SearchQuery(x => x.Customer == true && !partner_ids.Contains(x.Id)).Select(x => new SaleReportPartnerItem()
            {
                PartnerId = x.Id,
                OrderCount = 0
            }).ToListAsync();
            data.AddRange(partnersLast);
            var result = new List<SaleReportPartnerItem>();
            foreach (var item in data)
            {
                if (val.PartnerDisplay == "new" && item.OrderCount > 0)
                    continue;
                if (val.PartnerDisplay == "old" && item.OrderCount == 0)
                    continue;
                var partner = partnerDict[item.PartnerId];
                item.PartnerName = partner.Name;
                item.PartnerPhone = partner.Phone;
                item.PartnerDisplayName = partner.DisplayName;

                result.Add(item);
            }



            return result;
        }

        public async Task<IEnumerable<SaleReportPartnerItemV3>> GetReportPartnerV3(SaleReportPartnerSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var saleLineObj = GetService<ISaleOrderLineService>();
            var queryFilter = _context.SaleReports.Where(x => !x.CompanyId.HasValue || company_ids.Contains(x.CompanyId.Value));
            var query = _context.SaleReports.Where(x => !x.CompanyId.HasValue || company_ids.Contains(x.CompanyId.Value));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            var result = query.Include(x => x.Partner).AsEnumerable().GroupBy(x => new { PartnerId = x.PartnerId, OrderName = x.Name, PartnerName = x.Partner.DisplayName, Date = x.Date })
                .Select(x => new SaleReportPartnerV3Detail
                {
                    Date = x.Key.Date,
                    OrderName = x.Key.OrderName,
                    PartnerName = x.Key.PartnerName,
                    PartnerId = x.Key.PartnerId.Value,
                    CountLine = x.Count(),
                    Type = queryFilter.Where(s => s.PartnerId == x.Key.PartnerId && s.Date < DateUtils.FirstDateOfWeekISO8601(x.Key.Date.Year, GetIso8601WeekOfYear(x.Key.Date))).Count() >= 1 ? "KHC" : "KHM"
                })
                .GroupBy(x => new
                {
                    Year = x.Date.Year,
                    WeekStart = DateUtils.FirstDateOfWeekISO8601(x.Date.Year, GetIso8601WeekOfYear(x.Date)),
                    WeekEnd = DateUtils.FirstDateOfWeekISO8601(x.Date.Year, GetIso8601WeekOfYear(x.Date)).AddDays(6).AddHours(23).AddMinutes(59),
                    WeekOfYear = GetIso8601WeekOfYear(x.Date)
                })
                  .Select(x => new SaleReportPartnerItemV3
                  {
                      WeekOfYear = x.Key.WeekOfYear,
                      Year = x.Key.Year,
                      TotalNewPartner = x.Where(x => x.Type == "KHM").GroupBy(x => x.PartnerId).Count(),
                      TotalOldPartner = x.Where(x => x.Type == "KHC").GroupBy(x => x.PartnerId).Count(),
                      lines = x.ToList()
                  }).ToList();
            return result;
        }

        public async Task<IEnumerable<SaleReportPartnerItemV3>> GetReportPartnerV4(SaleReportPartnerSearch val)
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

            var result = query.AsEnumerable().GroupBy(x => new
            {
                Year = x.Date.Year,
                WeekOfYear = GetIso8601WeekOfYear(x.Date)
            })
                  .Select(x => new SaleReportPartnerItemV3
                  {
                      WeekOfYear = x.Key.WeekOfYear,
                      Year = x.Key.Year,
                      TotalNewPartner = x.Where(x => x.Type == "KHM").Count(),
                      TotalOldPartner = x.Where(x => x.Type == "KHC").Count(),
                      lines = _mapper.Map<IEnumerable<SaleReportPartnerV3Detail>>(x.ToList()),
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

        public async Task<IEnumerable<dynamic>> GetWeeksOfYear()
        {
            var jan1 = new DateTime(DateTime.Today.Year, 1, 1);
            //beware different cultures, see other answers
            var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            var weeks = Enumerable.Range(0, 54).Select(i => new { weekStart = startOfFirstWeek.AddDays(i * 7) })
                    .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                    .Select(x => new { x.weekStart, weekFinish = x.weekStart.AddDays(6) })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new { x.weekStart, x.weekFinish, weekNum = i + 1 });

            return weeks;
        }

        public async Task<IEnumerable<SaleReportItem>> GetTopSaleProduct(SaleReportTopSaleProductSearch val)
        {
            //thời gian, tiền thực thu, doanh thu, còn nợ
            var companyId = CompanyId;
            var states = new string[] { "sale", "done" };
            var query = _context.SaleReports.Where(x => x.CompanyId == companyId && states.Contains(x.State) &&
            (!x.IsQuotation.HasValue || x.IsQuotation == false) && x.Product.SaleOK == true);
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

            if (val.TopBy == "amount")
            {
                var result = await query.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                })
                   .Select(x => new SaleReportItem
                   {
                       ProductId = x.Key.ProductId,
                       Name = x.Key.ProductName,
                       PriceTotal = x.Sum(s => s.PriceTotal),
                   }).OrderByDescending(x => x.PriceTotal).Take(10).ToListAsync();
                return result;
            }
            else
            {
                var result = await query.GroupBy(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                })
                     .Select(x => new SaleReportItem
                     {
                         ProductId = x.Key.ProductId,
                         Name = x.Key.ProductName,
                         ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                     }).OrderByDescending(x => x.ProductUOMQty).Take(10).ToListAsync();
                return result;
            }
        }

        public async Task<SaleReportOldNewPartnerOutput> GetReportOldNewPartner(SaleReportOldNewPartnerInput val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();

            //truy van so tong benh nhan
            var query = _context.SaleReports.Where(x => !x.CompanyId.HasValue || company_ids.Contains(x.CompanyId.Value));
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            IEnumerable<Guid?> idsKhachHangTruocKhoang = null;

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.Date >= dateFrom);

                var query2 = query.Where(x => x.Date < dateFrom);
                idsKhachHangTruocKhoang = await query2.GroupBy(x => x.PartnerId).Select(x => x.Key).ToListAsync();
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateTo);
            }

            var idsKhachHangTrongKhoang = await query.GroupBy(x => x.PartnerId).Select(x => x.Key).ToListAsync();
            var tongBenhNhan = idsKhachHangTrongKhoang.Count;
            int soCu = 0;
            if (idsKhachHangTruocKhoang != null)
            {
                soCu = idsKhachHangTrongKhoang.Where(x => !idsKhachHangTruocKhoang.Contains(x)).Count();
            }

            int soMoi = tongBenhNhan - soCu;





            return null;
        }

        public async Task<IEnumerable<SaleReportItem>> GetReportForSmsMessage(IEnumerable<Guid> partnerIds)
        {
            var query = _context.SaleReports.Where(x => x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value)).AsQueryable();
            var result = await query.GroupBy(x => new
            {
                PartnerId = x.PartnerId,
                PartnerName = x.Partner.Name
            })
                  .Select(x => new SaleReportItem
                  {
                      PartnerId = x.Key.PartnerId,
                      Name = x.Key.PartnerName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      CompanyId = CompanyId
                  }).ToListAsync();
            return result;
        }

        private IQueryable<SaleOrderLine> GetServiceReportQuery(ServiceReportReq val)
        {
            var orderLineObj = GetService<ISaleOrderLineService>();
            var query = orderLineObj.SearchQuery(x=> x.State == "sale" || x.State == "done");
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value.AbsoluteEndOfDate());

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            if(val.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == val.EmployeeId);

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if(val.Active.HasValue)
                query = query.Where(x => x.IsActive == val.Active);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.OrderPartner.Name.Contains(val.Search) || x.OrderPartner.Name.Contains(val.Search)
                                         || x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search));

            return query;
        }
        public async Task<IEnumerable<ServiceReportRes>> GetServiceReportByTime(ServiceReportReq val)
        {
            var res = await GetServiceReportQuery(val).GroupBy(x => x.Date).Select(x=> new ServiceReportRes() { 
            Date = x.Key, 
            Quantity = x.Count(),
            TotalAmount = x.Sum(z=> z.PriceSubTotal)
            }).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<ServiceReportRes>> GetServiceReportByService(ServiceReportReq val)
        {
            var res = await GetServiceReportQuery(val).GroupBy(x => new { x.ProductId, x.Name }).Select(x => new ServiceReportRes()
            {   
                Name = x.Key.Name,
                Quantity = x.Count(),
                TotalAmount = x.Sum(z => z.PriceSubTotal),
                ProductId = x.Key.ProductId
            }).ToListAsync();

            return res;
        }

        public async Task<PagedResult2<ServiceReportDetailRes>> GetServiceReportDetailPaged(ServiceReportDetailReq val)
        {
            var query = GetServiceReportQuery(new ServiceReportReq(){
            CompanyId = val.CompanyId,
            DateFrom = val.DateFrom,
            DateTo = val.DateTo,
            EmployeeId = val.EmployeeId,
            Search = val.Search,
            State = val.State
            });

            if (val.ProductId.HasValue)
                query = query.Where(x => x.ProductId == val.ProductId);
            var count = await query.CountAsync();
            if(val.Limit > 0)
            query = query.Skip(val.Offset).Take(val.Limit);

            var res = await _mapper.ProjectTo<ServiceReportDetailRes>(query).ToListAsync();
            return new PagedResult2<ServiceReportDetailRes>(count, val.Offset, val.Limit) { 
            Items = res
            };
        }
    }
}
