using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class CommissionReportService : ICommissionReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommissionReportService(CatalogDbContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<CommissionReport>> GetReport(ReportFilterCommission val)
        {
            var companyId = val.CompanyId.HasValue ? val.CompanyId : CompanyId;
            //SearchQuery
            var paymmentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var saleOrderObj = GetService<IAccountPaymentService>();
            var saleOrderlinePartnerObj = GetService<ISaleOrderLinePartnerCommissionService>();
            var paymentRels = paymmentRelObj.SearchQuery();

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                paymentRels = paymentRels.Where(x => x.Payment.PaymentDate >= dateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                paymentRels = paymentRels.Where(x => x.Payment.PaymentDate <= dateTo);
            }

            if (!string.IsNullOrWhiteSpace(val.UserId))
                paymentRels = paymentRels.Where(x => x.SaleOrderLine.SalesmanId == val.UserId);


            var list = await paymentRels.Select(x => x.SaleOrderLineId).ToListAsync();

            var lines = await saleOrderlinePartnerObj.SearchQuery(x => list.Contains(x.SaleOrderLineId)).Include(x => x.Partner).Include(x => x.Commission)
                .Include(x => x.SaleOrderLine)
                .Include("SaleOrderLine.Salesman")
                .Include("SaleOrderLine.Product")
                .Include("SaleOrderLine.SaleOrderLinePaymentRels")
               .ToListAsync();


            var res = lines.Select(x => new CommissionReportItem
            {
                UserId = x.SaleOrderLine.SalesmanId,
                Name = x.SaleOrderLine.Salesman.Name,
                ProductName = x.SaleOrderLine.Product.Name,
                AmountTotal = x.SaleOrderLine.PriceTotal,
                PrepaidTotal = x.SaleOrderLine.SaleOrderLinePaymentRels.Sum(s => s.AmountPrepaid),
                PercentCommission = x.Percentage,
                EstimateTotal = x.Amount ?? 0,
                CommissionTotal = (x.SaleOrderLine.AmountPaid * x.Percentage) / 100
            }).ToList();

            var res2 = res.GroupBy(x => new
            {
                UserId = x.UserId,
                UserName = x.Name
            }).Select(x => new CommissionReport
            {
                UserId = x.Key.UserId,
                Name = x.Key.UserName,
                EstimateTotal = x.Sum(s=>s.EstimateTotal),
                CommissionTotal = x.Sum(s => s.CommissionTotal)
            }).ToList();

            return res2;


        }


        public async Task<IEnumerable<CommissionReportItem>> GetReportDetail(ReportFilterCommissionDetail val)
        {
            var companyId = CompanyId;
            //SearchQuery
            var paymmentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            var saleOrderObj = GetService<IAccountPaymentService>();
            var saleOrderlinePartnerObj = GetService<ISaleOrderLinePartnerCommissionService>();
            var paymentRels = paymmentRelObj.SearchQuery(x => x.AmountPrepaid != 0);

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                paymentRels = paymentRels.Where(x => x.Payment.PaymentDate >= dateFrom);
            }
            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                paymentRels = paymentRels.Where(x => x.Payment.PaymentDate <= dateTo);
            }


            var list = await paymentRels.Select(x => x.SaleOrderLineId).ToListAsync();

            var lines = await saleOrderlinePartnerObj.SearchQuery(x => list.Contains(x.SaleOrderLineId) && x.SaleOrderLine.SalesmanId == val.UserId).Include(x => x.Partner).Include(x => x.Commission)
              .Include(x => x.SaleOrderLine)
              .Include("SaleOrderLine.Salesman")
              .Include("SaleOrderLine.Product")
              .Include("SaleOrderLine.SaleOrderLinePaymentRels")
             .ToListAsync();


            var res = lines.Select(x => new CommissionReportItem
            {
                UserId = x.SaleOrderLine.SalesmanId,
                Name = x.SaleOrderLine.Salesman.Name,
                ProductName = x.SaleOrderLine.Product.Name,
                Date = x.LastUpdated,
                AmountTotal = x.SaleOrderLine.PriceTotal,
                PrepaidTotal = x.SaleOrderLine.AmountPaid,
                PercentCommission = x.Percentage,
                EstimateTotal = x.Amount ?? 0,
                CommissionTotal = (x.SaleOrderLine.AmountPaid * x.Percentage) /100, 
            }).OrderByDescending(x => x.Date).ToList();

          
            return res;


        }


    }


   
}
