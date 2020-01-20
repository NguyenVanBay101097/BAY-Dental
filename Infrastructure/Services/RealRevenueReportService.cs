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
            //thời gian, tiền thực thu, doanh thu, còn nợ
            var companyId = CompanyId;
            var amlObj = GetService<IAccountMoveLineService>();
            var query = amlObj._QueryGet(state: "posted", companyId: companyId);
            query = query.Where(x => x.Account.InternalType == "receivable" && !x.FullReconcileId.HasValue);

            var result = await query.GroupBy(x => 0).Select(x => new RealRevenueReportResult
            {
                Debit = x.Sum(s => s.Debit),
                Credit = x.Sum(s => s.Credit),
                Balance = x.Sum(s => s.Debit - s.Credit)
            }).FirstOrDefaultAsync();

            if (result == null)
                return new RealRevenueReportResult();

            result.Items = await query.GroupBy(x => new {
                PartnerId = x.PartnerId,
                PartnerName = x.Partner.Name,
            })
                  .Select(x => new RealRevenueReportItem
                  {
                      Name = x.Key.PartnerName,
                      Debit = x.Sum(s => s.Debit),
                      Credit = x.Sum(s => s.Credit),
                      Balance = x.Sum(s => s.Debit - s.Credit),
                  }).ToListAsync();
            return result;
        }
    }
}
