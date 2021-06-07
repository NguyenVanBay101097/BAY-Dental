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
            var query = movelineObj._QueryGet(companyId: val.CompanyId, state: "posted", dateFrom: val.DateFrom,
                dateTo: val.DateTo);

            query = query.Where(x => x.Account.Code == "CNKH");

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Move.InvoiceOrigin.Contains(val.Search));

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);


            var items = await query.Select(x => new CustomerDebtResult
            {
                Name = x.Payment != null ? x.Payment.Communication : x.PhieuThuChi.Reason,
                Date = x.Date,
                Balance = x.Balance,
                InvoiceOrigin = x.Move.InvoiceOrigin,
            }).ToListAsync();

            var paged = new PagedResult2<CustomerDebtResult>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

            return paged;
        }

        public async Task<CustomerDebtAmountTotal> GetCustomerAmountTotal(AmountCustomerDebtFilter val)
        {
            var movelineObj = GetService<IAccountMoveLineService>();
            var query = movelineObj._QueryGet(companyId: val.CompanyId, state: "posted", dateFrom: null,
           dateTo: null);

            query = query.Where(x => x.Account.Code == "CNKH");

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);

            var res = await query.GroupBy(x => x.PartnerId.Value).Select(x => new CustomerDebtAmountTotal
            {
                DebitTotal = x.Sum(x => x.Debit),
                CreditTotal = x.Sum(x => x.Credit),
                BalanceTotal = x.Sum(x => x.Balance)
            }).FirstOrDefaultAsync();

            var result = new CustomerDebtAmountTotal();
            result.DebitTotal = res != null ? res.DebitTotal : 0;
            result.CreditTotal = res != null ? res.CreditTotal : 0;
            result.BalanceTotal = res != null ? res.BalanceTotal : 0;

            return result;
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

        public Guid? CompanyId { get; set; }

    }


    public class CustomerDebtResult
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public string InvoiceOrigin { get; set; }

        public decimal Balance { get; set; }
    }

    public class AmountCustomerDebtFilter
    {
        public Guid? PartnerId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class CustomerDebtAmountTotal
    {
        public decimal DebitTotal { get; set; }
        public decimal CreditTotal { get; set; }
        public decimal BalanceTotal { get; set; }
    }
}
