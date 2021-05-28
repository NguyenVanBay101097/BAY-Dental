using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
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
    public class AccountInvoiceReportService : BaseService<Product>, IAccountInvoiceReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        public AccountInvoiceReportService(CatalogDbContext context, IAsyncRepository<Product> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
        }

        private async Task<IQueryable<AccountInvoiceReport>> GetRevenueReportQuery(AccountInvoiceReportPaged val)
        {

            var accObj = GetService<IAccountAccountService>();
            var accounts = await accObj.SearchQuery(x => x.Code == "5111" && (val.CompanyId.HasValue ? x.CompanyId == val.CompanyId : true)).ToListAsync();
            if (accounts.Count == 0)
            {
                throw new Exception("Không tồn tại tài khoản doanh thu");
            }

            var accIds = accounts.Select(x => x.Id).ToList();
            var query = _context.AccountInvoiceReports.AsQueryable();
            query = query.Where(x => accIds.Any(idacc => idacc == x.AccountId.Value));

            if (val.DateFrom.HasValue)
            {
                var dateFrom = new DateTime(val.DateFrom.Value.Year, val.DateFrom.Value.Month, val.DateFrom.Value.Day, 0, 0, 0, 0);
                query = query.Where(x => x.InvoiceDate >= dateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = new DateTime(val.DateTo.Value.Year, val.DateTo.Value.Month, val.DateTo.Value.Day, 23, 59, 59, 999);
                query = query.Where(x => x.InvoiceDate >= dateTo);
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            }

            return query;
        }

        public async Task<PagedResult2<AccountInvoiceReportDisplay>> GetRevenueReportPaged(AccountInvoiceReportPaged val)
        {
            var query = await GetRevenueReportQuery(val);

            var res = new List<AccountInvoiceReportDisplay>();
            var count = 0;
            switch (val.GroupBy)
            {
                case "InvoiceDate":
                    var queryDate = query.GroupBy(x => x.InvoiceDate.Value.Date);
                    count = await queryDate.Select(x => x.Key).CountAsync();
                    if (val.Offset > 0) queryDate.Skip(val.Offset).Take(val.Limit);
                    res = await queryDate.Select(x => new AccountInvoiceReportDisplay
                    {
                        InvoiceDate = x.Key as DateTime?,
                        PriceSubTotal = x.Sum(z => z.PriceSubTotal)
                    }).ToListAsync();
                    break;

                case "ProductId":
                    var queryPr = query.GroupBy(x => new { x.ProductId, x.Product.Name });
                    count = await queryPr.Select(x => x.Key).CountAsync();
                    if (val.Offset > 0) queryPr.Skip(val.Offset).Take(val.Limit);
                    res = await queryPr.Select(x => new AccountInvoiceReportDisplay
                    {
                        ProductId = x.Key.ProductId.Value,
                        ProductName = x.Key.Name,
                        PriceSubTotal = x.Sum(z => z.PriceSubTotal)
                    }).ToListAsync();
                    break;
                case "EmployeeId":
                    var queryEmp = query.GroupBy(x => new { x.EmployeeId, x.Employee.Name });
                    count = await queryEmp.Select(x => x.Key).CountAsync();
                    if (val.Offset > 0) queryEmp.Skip(val.Offset).Take(val.Limit);
                    res = await queryEmp.Select(x => new AccountInvoiceReportDisplay
                    {
                        EmployeeId = x.Key.EmployeeId,
                        EmployeeName = x.Key.Name,
                        PriceSubTotal = x.Sum(z => z.PriceSubTotal)
                    }).ToListAsync();
                    break;
                case "AssistantId":
                    var queryAss = query.GroupBy(x => new { x.AssistantId, x.Assistant.Name });
                    count = await queryAss.Select(x => x.Key).CountAsync();
                    if (val.Offset > 0) queryAss.Skip(val.Offset).Take(val.Limit);
                    res = await queryAss.Select(x => new AccountInvoiceReportDisplay
                    {
                        AssistantId = x.Key.AssistantId,
                        AssistantName = x.Key.Name,
                        PriceSubTotal = x.Sum(z => z.PriceSubTotal)
                    }).ToListAsync();
                    break;
                default:
                    break;
            }

            return new PagedResult2<AccountInvoiceReportDisplay>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        public async Task<PagedResult2<AccountInvoiceReportDetailDisplay>> GetRevenueReportDetailPaged(AccountInvoiceReportDetailPaged val)
        {
            var query = await GetRevenueReportQuery(new AccountInvoiceReportPaged()
            {
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
            });

            var res = new List<AccountInvoiceReportDetailDisplay>();
            var count = 0;

            if (val.Date.HasValue)
            {
                query = query.Where(x => x.InvoiceDate.Value.Date == val.Date.Value.Date);
            }
            else if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            else if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }
            else if (val.AssistantId.HasValue)
            {
                query = query.Where(x => x.AssistantId == val.AssistantId);
            }

            count = await query.CountAsync();
            if (val.Offset > 0) query = query.Skip(val.Offset).Take(val.Limit);
            res = await query.Select(x => new AccountInvoiceReportDetailDisplay
            {
                InvoiceDate = x.InvoiceDate,
                AssistantName = x.Assistant.Name,
                EmployeeName = x.Employee.Name,
                InvoiceOrigin = x.InvoiceOrigin,
                PartnerName = x.Partner.Name,
                PriceSubTotal = x.PriceSubTotal,
                ProductName = x.Product.Name
            }).ToListAsync();

            return new PagedResult2<AccountInvoiceReportDetailDisplay>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        //public override ISpecification<AccountInvoiceReport> RuleDomainGet(IRRule rule)
        //{
        //    var companyId = CompanyId;
        //    switch (rule.Code)
        //    {
        //        case "account.invoice_comp_rule":
        //            return new InitialSpecification<AccountInvoiceReport>(x => x.company_id == companyId);
        //        default:
        //            return null;
        //    }
        //}
    }
}
