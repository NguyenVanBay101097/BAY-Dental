using ApplicationCore.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CommissionReportService : ICommissionReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommissionReportService(IHttpContextAccessor httpContextAccessor)
        {
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

        public async Task<IEnumerable<CommissionReport>> GetReport(ReportFilterCommission val)
        {
            var companyId = CompanyId;
            //SearchQuery
            var paymmentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var saleOrderObj = GetService<IAccountPaymentService>();
            var saleOrderlineObj = GetService<ISaleOrderLineService>();
            var payments = saleOrderObj.SearchQuery();

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                payments = payments.Where(x => x.PaymentDate >= dateFrom);
            }
            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                payments = payments.Where(x => x.PaymentDate <= dateTo);
            }

            var list = await payments.ToListAsync();

            var lines = await saleOrderlineObj.SearchQuery().Include(x => x.Salesman).Include(x=>x.PartnerCommission)
                .Include("Salesman.Partner")
                .Include("Salesman.Employee")
                .Include("Salesman.Employee.Commission").ToListAsync();

            var res = lines.Select(x => new CommissionReport
            {
                UserId = x.SalesmanId,
                Name = x.Salesman.Name,
                AmountTotal = x.PriceSubTotal,
                PrepaidTotal = x.SaleOrderLinePaymentRels.Sum(s=>s.AmountPrepaid),
                ProductName = x.Name,
                CommissionTotal = 0
            }).ToList();

            return res;
        }



    }

    public class CommissionReport
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// dịch vụ
        /// </summary>
        public string ProductName { get; set; }

        public decimal? AmountTotal { get; set; }
        public decimal? PrepaidTotal { get; set; }
        public decimal? CommissionTotal { get; set; }
    }

    public class ReportFilterCommission
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
