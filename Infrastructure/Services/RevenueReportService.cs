using ApplicationCore.Entities;
using ApplicationCore.Utilities;
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
    public class RevenueReportService: IRevenueReportService
    {
        private readonly CatalogDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public RevenueReportService(CatalogDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
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

        public async Task<RevenueReportResult> GetReport(RevenueReportSearch val)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var account_type_revenue = await modelDataObj.GetRef<AccountAccountType>("account.data_account_type_revenue");

            if (val.DateFrom.HasValue)
                val.DateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
            if (val.DateTo.HasValue)
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();

            var companyId = CompanyId;

            if (val.CompanyId.HasValue)
                companyId = val.CompanyId.Value;

            var query = amlObj._QueryGet(dateTo: val.DateTo, dateFrom: val.DateFrom, state: "posted", companyId: companyId);
            query = query.Where(x => x.Account.UserTypeId == account_type_revenue.Id);
            if (!string.IsNullOrEmpty(val.Search))
            {
                if (val.GroupBy == "partner")
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search));
                else if (val.GroupBy == "product")
                    query = query.Where(x => x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search));
            }

            var result = await query.GroupBy(x => 0).Select(x => new RevenueReportResult
            {
                Debit = x.Sum(s => s.Debit),
                Credit = x.Sum(s => s.Credit),
                Balance = x.Sum(s => s.Credit - s.Debit)
            }).FirstOrDefaultAsync();

            if (result == null)
                return new RevenueReportResult();

            var companyObj = GetService<ICompanyService>();
            result.Company = (await companyObj.GetPagedResultAsync(new CompanyPaged() { Search = "" })).Items;

            if (val.GroupBy == "partner")
            {
                result.Details = await query.GroupBy(x => new { x.PartnerId, x.Partner.Name }).Select(x => new RevenueReportResultDetail
                {
                    Name = x.Key.Name,
                    Debit = x.Sum(s => s.Debit),
                    Credit = x.Sum(s => s.Credit),
                    Balance = x.Sum(s => s.Credit - s.Debit)
                }).ToListAsync();
            }
            if (val.GroupBy == "product")
            {
                result.Details = await query.GroupBy(x => new { x.ProductId, x.Product.Name }).Select(x => new RevenueReportResultDetail
                {
                    Name = x.Key.Name,
                    Debit = x.Sum(s => s.Debit),
                    Credit = x.Sum(s => s.Credit),
                    Balance = x.Sum(s => s.Credit - s.Debit)
                }).ToListAsync();
            }
            if (val.GroupBy == "date:quarter")
            {
                result.Details = await query.GroupBy(x => new {
                    x.Date.Value.Year,
                    QuarterOfYear = (x.Date.Value.Month - 1) / 3,
                })
                  .Select(x => new RevenueReportResultDetail
                  {
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Credit - s.Debit),
                      Year = x.Key.Year,
                      QuarterOfYear = x.Key.QuarterOfYear,
                  }).ToListAsync();

                foreach (var item in result.Details)
                    item.Name = $"{item.QuarterOfYear + 1}, {item.Year}";
            }
            else if (val.GroupBy == "date:month" || val.GroupBy == "date")
            {
                result.Details = await query.GroupBy(x => new {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                })
                  .Select(x => new RevenueReportResultDetail
                  {
                      Year = x.Key.Year,
                      Month = x.Key.Month,
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Credit - s.Debit),
                  }).ToListAsync();

                foreach (var item in result.Details)
                    item.Name = new DateTime(item.Year, item.Month, 1).ToString("MM/yyyy");
            }
            if (val.GroupBy == "date:week")
            {
                result.Details = query.GroupBy(x => _context.DatePart("week", x.Date))
                  .Select(x => new RevenueReportResultDetail
                  {
                      WeekOfYear = x.Key,
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Credit - s.Debit)
                  }).ToList();

                foreach (var item in result.Details)
                    item.Name = $"{item.WeekOfYear}";
            }
            else if (val.GroupBy == "date:day")
            {
                result.Details = await query.GroupBy(x => new {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                    x.Date.Value.Day,
                })
                   .Select(x => new RevenueReportResultDetail
                   {
                       Year = x.Key.Year,
                       Month = x.Key.Month,
                       Day = x.Key.Day,
                       Debit = x.Sum(s => s.Debit),
                       Credit = x.Sum(s => s.Credit),
                       Balance = x.Sum(s => s.Credit - s.Debit)
                   }).ToListAsync();

                foreach (var item in result.Details)
                    item.Name = new DateTime(item.Year, item.Month, item.Day).ToString("dd/MM/yyyy");
            }
            else if (val.GroupBy == "date:year")
            {
                result.Details = await query.GroupBy(x => new {
                    x.Date.Value.Year,
                })
                   .Select(x => new RevenueReportResultDetail
                   {
                       Year = x.Key.Year,
                       Debit = x.Sum(s => s.Debit),
                       Credit = x.Sum(s => s.Credit),
                       Balance = x.Sum(s => s.Credit - s.Debit)
                   }).ToListAsync();

                foreach (var item in result.Details)
                    item.Name = new DateTime(item.Year, 1, 1).ToString("yyyy");
            }
            if (val.GroupBy == "salesman")
            {
                result.Details = await query.GroupBy(x => new { 
                    x.SalesmanId, x.Salesman.Name 
                })
                    .Select(x => new RevenueReportResultDetail
                    {
                        Name = x.Key.Name,
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                        Balance = x.Sum(s => s.Credit - s.Debit)
                    }).ToListAsync();

            }

            return result;
        }
    }
}
