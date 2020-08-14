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
            //var companyId = val.CompanyId.HasValue ? val.CompanyId : CompanyId;
            ////SearchQuery
            //var paymmentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            //var saleOrderObj = GetService<IAccountPaymentService>();
            //var saleOrderlineObj = GetService<ISaleOrderLineService>();
            //var paymentRels = paymmentRelObj.SearchQuery(x => x.AmountPrepaid != 0);

            //if (val.DateFrom.HasValue)
            //{
            //    var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
            //    paymentRels = paymentRels.Where(x => x.Payment.PaymentDate >= dateFrom);
            //}

            //if (val.DateTo.HasValue)
            //{
            //    var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
            //    paymentRels = paymentRels.Where(x => x.Payment.PaymentDate <= dateTo);
            //}



            //var list = await paymentRels.Select(x => x.SaleOrderLineId).ToListAsync();

            //var lines = await saleOrderlineObj.SearchQuery(x => list.Contains(x.Id) && x.CompanyId == companyId).Include(x => x.Salesman).Include(x => x.PartnerCommission).Include(x => x.SaleOrderLinePaymentRels)
            //    .Include("Salesman.Partner")
            //    .Where(x => x.PartnerCommission.CommissionId != null).ToListAsync();

            //var res = lines.Select(x => new CommissionReportItem
            //{
            //    UserId = x.SalesmanId,
            //    Name = x.Salesman.Name,
            //    AmountTotal = x.PriceSubTotal,
            //    PrepaidTotal = x.SaleOrderLinePaymentRels.Sum(s => s.AmountPrepaid),
            //    ProductName = x.Name,
            //    PercentCommission = AddCommissionSaleOrderLine(x.PartnerCommission.CommissionId.Value, x.ProductId.Value),
            //    CommissionTotal = _ComputeCommission(x)
            //}).ToList();

            //var res2 = res.GroupBy(x => new
            //{
            //    UserId = x.UserId,
            //    UserName = x.Name
            //}).Select(x => new CommissionReport
            //{
            //    UserId = x.Key.UserId,
            //    Name = x.Key.UserName,
            //    CommissionTotal = x.Sum(s => s.CommissionTotal)
            //}).ToList();

            //return res2;

            return null;
        }

        public async Task<IEnumerable<CommissionReportItem>> GetReportDetail(ReportFilterCommissionDetail val)
        {
            //var companyId = CompanyId;
            ////SearchQuery
            //var paymmentRelObj = GetService<ISaleOrderLinePaymentRelService>();
            //var saleOrderObj = GetService<IAccountPaymentService>();
            //var saleOrderlineObj = GetService<ISaleOrderLineService>();
            //var paymentRels = paymmentRelObj.SearchQuery(x => x.AmountPrepaid != 0);

            //if (val.DateFrom.HasValue)
            //{
            //    var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
            //    paymentRels = paymentRels.Where(x => x.Payment.PaymentDate >= dateFrom);
            //}
            //if (val.DateTo.HasValue)
            //{
            //    var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
            //    paymentRels = paymentRels.Where(x => x.Payment.PaymentDate <= dateTo);
            //}


            //var list = await paymentRels.Select(x => x.SaleOrderLineId).ToListAsync();

            //var lines = await saleOrderlineObj.SearchQuery(x => list.Contains(x.Id) && x.SalesmanId == val.UserId).Include(x => x.Salesman).Include(x => x.PartnerCommission).Include(x => x.SaleOrderLinePaymentRels).Include("SaleOrderLinePaymentRels.Payment")
            //    .Include("Salesman.Partner")
            //    .ToListAsync();

            //var res = lines.Select(x => new CommissionReportItem
            //{
            //    UserId = x.SalesmanId,
            //    Name = x.Salesman.Name,
            //    Date = x.SaleOrderLinePaymentRels.Max(s => s.Payment.PaymentDate),
            //    AmountTotal = x.PriceSubTotal,
            //    PrepaidTotal = x.SaleOrderLinePaymentRels.Sum(s => s.AmountPrepaid),
            //    ProductName = x.Name,
            //    PercentCommission = AddCommissionSaleOrderLine(x.PartnerCommission.CommissionId.Value, x.ProductId.Value),
            //    CommissionTotal = _ComputeCommission(x)
            //}).ToList();


            //return res;

            return null;
        }



        public decimal AddCommissionSaleOrderLine(Guid commissionId, Guid productId)
        {
            var commissionObj = GetService<ICommissionService>();
            var productObj = GetService<IProductService>();
            decimal percent = 0;

            var product = _context.Products.Where(x => x.CompanyId == CompanyId && x.Id == productId).FirstOrDefault();
            var rules = _context.CommissionProductRules.Where(x => x.CommissionId == commissionId).ToList();
            foreach (var rule in rules)
            {
                if (rule.AppliedOn == "2_product_category")
                {
                    if (product.CategId != rule.CategId)
                        continue;

                    percent = rule.PercentFixed.Value;
                }
                else if (rule.AppliedOn == "0_product_variant")
                {
                    if (rule.ProductId != productId)
                        continue;

                    percent = rule.PercentFixed.Value;
                }
                else
                {
                    percent = rule.PercentFixed.Value;
                }

            }

            return percent;
        }

        public decimal _ComputeCommission(SaleOrderLine val)
        {
            //var PrepaidTotal = val.SaleOrderLinePaymentRels.Sum(s => s.AmountPrepaid);
            //var PercentCommission = AddCommissionSaleOrderLine(val.PartnerCommission.CommissionId.Value, val.ProductId.Value);
            //var res = (PrepaidTotal.Value * PercentCommission) / 100;
            //return res;
            return 0M;
        }

    }


    public class CommissionReport
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public decimal? CommissionTotal { get; set; }

    }

    public class CommissionReportItem
    {
        public string UserId { get; set; }
        public string Name { get; set; }

        public DateTime? Date { get; set; }
        /// <summary>
        /// dịch vụ
        /// </summary>
        public string ProductName { get; set; }

        public decimal? AmountTotal { get; set; }
        public decimal? PrepaidTotal { get; set; }
        public decimal? PercentCommission { get; set; }
        public decimal? CommissionTotal { get; set; }
    }

    public class ReportFilterCommission
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class ReportFilterCommissionDetail
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string UserId { get; set; }
    }
}
