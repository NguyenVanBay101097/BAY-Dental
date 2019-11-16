using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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


        public async Task<IEnumerable<SaleReportTopServicesCs>> GetTopServices(SaleReportTopServicesFilter val)
        {
            var query = _context.SaleReports.AsQueryable();
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
            var companyId = CompanyId;
            var query = _context.SaleReports.Where(x => x.CompanyId == companyId);
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
                query = query.Where(x => x.State == val.State);

            if (val.GroupBy == "customer")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));

                var result = await query.GroupBy(x => new {
                    PartnerId = x.PartnerId,
                    PartnerName = x.Partner.Name
                })
                  .Select(x => new SaleReportItem
                  {
                      PartnerId = x.Key.PartnerId,
                      Name = x.Key.PartnerName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                return result;
            }
            if (val.GroupBy == "user")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.User.Name.Contains(val.Search));

                var result = await query.GroupBy(x => new {
                    UserId = x.UserId,
                    UserName = x.User.Name
                })
                  .Select(x => new SaleReportItem
                  {
                      UserId = x.Key.UserId,
                      Name = x.Key.UserName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                return result;
            }
            if (val.GroupBy == "product")
            {
                if (!string.IsNullOrEmpty(val.Search))
                    query = query.Where(x => x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search) ||
                    x.Product.DefaultCode.Contains(val.Search));

                var result = await query.GroupBy(x => new {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                })
                  .Select(x => new SaleReportItem
                  {
                      ProductId = x.Key.ProductId,
                      Name = x.Key.ProductName,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                return result;
            }
            if (val.GroupBy == "date:quarter")
            {
                var result = await query.GroupBy(x => new {
                    x.Date.Year,
                    QuarterOfYear = (x.Date.Month - 1) / 3,
                })
                  .Select(x => new SaleReportItem
                  {
                      Year = x.Key.Year,
                      QuarterOfYear = x.Key.QuarterOfYear,
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy
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
                var result = await query.GroupBy(x => new {
                    x.Date.Year,
                    x.Date.Month,
                })
                  .Select(x => new SaleReportItem
                  {
                      Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                      ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                      PriceTotal = x.Sum(s => s.PriceTotal),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                foreach (var item in result)
                {
                    item.Name = item.Date.Value.ToString("MM/yyyy");
                }
                return result;
            }
            if (val.GroupBy == "date:week")
            {
                var result = query.AsEnumerable().GroupBy(x => new {
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
                      GroupBy = val.GroupBy
                  }).ToList();
                foreach (var item in result)
                {
                    item.Date = DateUtils.FirstDateOfWeekISO8601(item.Year.Value, item.WeekOfYear.Value);
                    item.Name = $"Tuần {item.WeekOfYear}, {item.Year}";
                }
                return result;
            }
            else
            {
                var result = await query.GroupBy(x => new {
                    x.Date.Year,
                    x.Date.Month,
                    x.Date.Day,
                })
                   .Select(x => new SaleReportItem
                   {
                       Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                       ProductUOMQty = x.Sum(s => s.ProductUOMQty),
                       PriceTotal = x.Sum(s => s.PriceTotal),
                       GroupBy = val.GroupBy
                   }).ToListAsync();
                foreach (var item in result)
                {
                    item.Name = item.Date.Value.ToString("dd/MM/yyyy");
                }
                return result;
            }
        }

        public async Task<IEnumerable<SaleReportItemDetail>> GetReportDetail(SaleReportItem val)
        {
            var companyId = CompanyId;
            var query = _context.SaleReports.Where(x => x.CompanyId == companyId);
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
                query = query.Where(x => x.State == val.State);

            if (val.GroupBy == "customer")
                query = query.Where(x => x.PartnerId == val.PartnerId);
            else if (val.GroupBy == "user")
                query = query.Where(x => x.UserId == val.UserId);
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
    }
}
