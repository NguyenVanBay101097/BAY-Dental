using ApplicationCore.Models;
using ApplicationCore.Utilities;
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
    public class ReportCustomerDebtService : IReportCustomerDebtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReportCustomerDebtService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult2<CustomerDebtResult>> GetPagedtCustomerDebtReports(CustomerDebtFilter val)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();
            var accountDebt = await accountObj.GetAccountCustomerDebtCompany();

            var query = movelineObj.SearchQuery(x => x.AccountId == accountDebt.Id);

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.SaleLineRels.Any(x => x.OrderLine.Order.Name.Contains(val.Search)) || x.PhieuThuChi.Name.Contains(val.Search));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.PhieuThuChi).Include(x => x.Payment).OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);


            var payment_ids = query.Where(x => x.PaymentId.HasValue).Select(x => x.PaymentId.Value).ToList();
            var saleorder_dict = saleOrderPaymentObj.SearchQuery(x => x.PaymentRels.Any(s => payment_ids.Contains(s.PaymentId))).SelectMany(x => x.PaymentRels)
            .GroupBy(x => new { Id = x.PaymentId, SaleOrderId = x.SaleOrderPayment.OrderId, SaleOrderName = x.SaleOrderPayment.Order.Name }).Select(s => new
            {
                Id = s.Key.Id,
                OrderId = s.Key.SaleOrderId,
                OrderName = s.Key.SaleOrderName
            }).ToDictionary(x => x.Id, x => x);


            var items = await query.Select(x => new CustomerDebtResult
            {
                Id = x.PaymentId.HasValue ? saleorder_dict[x.PaymentId.Value].OrderId : x.PhieuThuChiId.Value,
                Name = x.PaymentId.HasValue ? saleorder_dict[x.PaymentId.Value].OrderName : x.PhieuThuChi.Name,
                PaymentName = x.Name,
                PaymentDate = x.Date.Value,
                PaymentAmount = x.Balance,
                Type = x.PaymentId.HasValue ? "debit" : "Credit",
            }).ToListAsync();

            var paged = new PagedResult2<CustomerDebtResult>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }


        public async Task<List<CustomerDebtResult>> GetExportExcel(CustomerDebtFilter val)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var accountObj = GetService<IAccountAccountService>();
            var saleOrderPaymentObj = GetService<ISaleOrderPaymentService>();
            var accountDebt = await accountObj.GetAccountCustomerDebtCompany();

            var query = movelineObj.SearchQuery(x => x.AccountId == accountDebt.Id);

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.SaleLineRels.Any(x => x.OrderLine.Order.Name.Contains(val.Search)));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.PhieuThuChi).Include(x => x.Payment).OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);


            var payment_ids = query.Where(x => x.PaymentId.HasValue).Select(x => x.PaymentId.Value).ToList();

            var saleorder_dict = saleOrderPaymentObj.SearchQuery(x => x.PaymentRels.Any(s => payment_ids.Contains(s.PaymentId))).SelectMany(x => x.PaymentRels)
            .GroupBy(x => new { Id = x.PaymentId, SaleOrderId = x.SaleOrderPayment.OrderId, SaleOrderName = x.SaleOrderPayment.Order.Name }).Select(s => new
            {
                Id = s.Key.Id,
                OrderId = s.Key.SaleOrderId,
                OrderName = s.Key.SaleOrderName
            }).ToDictionary(x => x.Id, x => x);

            var debts = await query.Select(x => new CustomerDebtResult
            {
                Id = x.PaymentId.HasValue ? saleorder_dict[x.PaymentId.Value].OrderId : x.PhieuThuChiId.Value,
                Name = x.PaymentId.HasValue ? saleorder_dict[x.PaymentId.Value].OrderName : x.PhieuThuChi.Name,
                PaymentName = x.Name,
                PaymentDate = x.Date.Value,
                PaymentAmount = x.Balance,
            }).ToListAsync();

            return debts;
        }



        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

    }

    public class CustomerDebtFilter
    {
        public CustomerDebtFilter()
        {
            Limit = 20;
        }
        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? PartnerId { get; set; }

    }


    public class CustomerDebtResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PaymentName { get; set; }
        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

        public string Type { get; set; }

    }
}
