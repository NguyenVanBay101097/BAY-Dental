using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;
using ApplicationCore.Utilities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Services
{
    /// <summary>
    /// Báo cáo khoản thu
    /// </summary>
    public class RealRevenueReportService: IRealRevenueReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RealRevenueReportService(CatalogDbContext context, IHttpContextAccessor httpContextAccessor)
        {
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

        public async Task<IEnumerable<RealRevenueReportItem>> GetReport(RealRevenueReportSearch val)
        {
            //thời gian, tiền thực thu, doanh thu, còn nợ
            var companyId = CompanyId;
            var query = _context.AccountMoveLines.Where(x => x.Account.InternalType == "receivable" && x.CompanyId == companyId);
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

            if (val.GroupBy == "partner")
            {
                var result = await query.GroupBy(x => new {
                    PartnerId = x.PartnerId,
                    PartnerName = x.Partner.Name,
                })
                  .Select(x => new RealRevenueReportItem
                  {
                      PartnerId = x.Key.PartnerId,
                      Name = x.Key.PartnerName,
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Debit - s.Credit),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                return result;
            }
            else if (val.GroupBy == "date:quarter")
            {
                var result = await query.GroupBy(x => new {
                    x.Date.Value.Year,
                    QuarterOfYear = (x.Date.Value.Month - 1) / 3,
                })
                  .Select(x => new RealRevenueReportItem
                  {
                      Year = x.Key.Year,
                      Date = new DateTime(x.Key.Year, x.Key.QuarterOfYear * 3 + 1, 1),
                      QuarterOfYear = x.Key.QuarterOfYear,
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Debit - s.Credit),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                foreach (var item in result)
                {
                    item.Name = $"Quý {item.QuarterOfYear}, {item.Year}";
                }
                return result;
            }
            else if (val.GroupBy == "date" || val.GroupBy == "date:month")
            {
                var result = await query.GroupBy(x => new {
                      x.Date.Value.Year,
                      x.Date.Value.Month,
                  })
                  .Select(x => new RealRevenueReportItem
                  {
                      Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Debit - s.Credit),
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
                var result = await query.GroupBy(x => new {
                    Year = x.Date.Value.Year,
                    WeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                        x.Date.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                })
                  .Select(x => new RealRevenueReportItem
                  {
                      Year = x.Key.Year,
                      WeekOfYear = x.Key.WeekOfYear,
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Debit - s.Credit),
                      GroupBy = val.GroupBy
                  }).ToListAsync();
                foreach(var item in result)
                {
                    item.Date = DateUtils.FirstDateOfWeekISO8601(item.Year.Value, item.WeekOfYear.Value);
                    item.Name = $"Tuần {item.WeekOfYear}, {item.Year}";
                }
                return result;
            }
            else
            {
                var result = await query.GroupBy(x => new {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                    x.Date.Value.Day,
                })
                   .Select(x => new RealRevenueReportItem
                   {
                       Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                       Debit = x.Sum(s => s.Debit),
                       Credit = x.Sum(s => s.Credit),
                       Balance = x.Sum(s => s.Debit - s.Credit),
                       GroupBy = val.GroupBy
                   }).ToListAsync();
                foreach (var item in result)
                {
                    item.Name = item.Date.Value.ToString("dd/MM/yyyy");
                }
                return result;
            }
        }

        public async Task<IEnumerable<RealRevenueReportItemDetail>> GetReportDetail(RealRevenueReportItem val)
        {
            var companyId = CompanyId;
            var query = _context.AccountMoveLines.Where(x => x.Account.InternalType == "receivable" && x.CompanyId == companyId);
            if (val.GroupBy == "partner")
                query = query.Where(x => x.PartnerId == val.PartnerId);
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

            var result = await query.Select(x => new RealRevenueReportItemDetail
            {
                Date = x.Date,
                Name = x.Name,
                Balance = x.Balance,
                Credit = x.Credit,
                Debit = x.Debit,
                Ref = x.Ref
            }).ToListAsync();
            return result;
        }
    }
}
