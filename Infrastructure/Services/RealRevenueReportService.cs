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

        public async Task<RealRevenueReportResult> GetReport(RealRevenueReportSearch val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var date_from = val.DateFrom;
            var date_to = val.DateTo.HasValue ? val.DateTo.Value.AbsoluteEndOfDate() : (DateTime?)null;
            var accountTypes = new string[] { "receivable" };
            decimal begin = 0;
            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, state: "posted", companyId: val.CompanyId);
                query = query.Where(x => accountTypes.Contains(x.Account.InternalType));
                begin = (await query.SumAsync(x => x.Debit - x.Credit));
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: val.CompanyId);
            query2 = query2.Where(x => accountTypes.Contains(x.Account.InternalType));

            var res = await query2.GroupBy(x => 0)
                    .Select(x => new RealRevenueReportResult
                    {
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                    }).FirstOrDefaultAsync();

            if (res == null)
                return res;

            res.Begin = begin;
            res.End = res.Begin + res.Debit - res.Credit;
            return res;
        }
    }
}
