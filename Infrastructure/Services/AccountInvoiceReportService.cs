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

        private async Task<IQueryable<AccountInvoiceReport>> GetRevenueReportQuery(RevenueReportQueryCommon val)
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
                query = query.Where(x => x.InvoiceDate <= dateTo);
            }

            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);
            }

            return query;
        }

        public async Task<PagedResult2<RevenueTimeReportDisplay>> GetRevenueTimeReportPaged(RevenueTimeReportPaged val)
        {
            var query = await GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            var queryRes = query.GroupBy(x => x.InvoiceDate.Value.Date);

            var res = new List<RevenueTimeReportDisplay>();
            var count = await queryRes.Select(x => x.Key).CountAsync();
            if (val.Limit > 0) queryRes = queryRes.Skip(val.Offset).Take(val.Limit);
            res = await queryRes.Select(x => new RevenueTimeReportDisplay
            {
                InvoiceDate = x.Key as DateTime?,
                PriceSubTotal = x.Sum(z => z.PriceSubTotal)
            }).ToListAsync();

            res = res.OrderByDescending(z => z.InvoiceDate.Value).ToList();

            return new PagedResult2<RevenueTimeReportDisplay>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        public async Task<PagedResult2<RevenueServiceReportDisplay>> GetRevenueServiceReportPaged(RevenueServiceReportPaged val)
        {
            var query = await GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            if(val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            var queryRes = query.GroupBy(x => new { x.ProductId, x.Product.Name});

            var res = new List<RevenueServiceReportDisplay>();
            var count = await queryRes.Select(x => x.Key).CountAsync();
            if (val.Limit > 0) queryRes = queryRes.Skip(val.Offset).Take(val.Limit);
            res = await queryRes.Select(x => new RevenueServiceReportDisplay
            {
                ProductId = x.Key.ProductId.Value,
                ProductName = x.Key.Name,
                PriceSubTotal = x.Sum(z => z.PriceSubTotal)
            }).ToListAsync();

            return new PagedResult2<RevenueServiceReportDisplay>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        public async Task<PagedResult2<RevenueEmployeeReportDisplay>> GetRevenueEmployeeReportPaged(RevenueEmployeeReportPaged val)
        {
            var query = await GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            IQueryable<IGrouping<object,AccountInvoiceReport>> queryRes;
            if(val.EmployeeGroup)
            {
                if (val.EmployeeId.HasValue)
                {
                    query = query.Where(x => x.EmployeeId == val.EmployeeId);
                }
                 queryRes = query.GroupBy(x => new EmployeeAssistantKeyGroup {Id = x.EmployeeId, Name = x.Employee.Name});
            }
            else
            {
                if (val.EmployeeId.HasValue)
                {
                    query = query.Where(x => x.AssistantId == val.EmployeeId);
                }
                 queryRes = query.GroupBy(x => new EmployeeAssistantKeyGroup { Id = x.AssistantId, Name = x.Assistant.Name });
            }

            var res = new List<RevenueEmployeeReportDisplay>();
            var count = await queryRes.Select(x => x.Key).CountAsync();
            if (val.Limit > 0) queryRes = queryRes.Skip(val.Offset).Take(val.Limit);
            res = await queryRes.Select(x => new RevenueEmployeeReportDisplay
            {
                EmployeeId = (x.Key as EmployeeAssistantKeyGroup).Id,
                EmployeeName = (x.Key as EmployeeAssistantKeyGroup).Name,
                PriceSubTotal = x.Sum(z => z.PriceSubTotal)
            }).ToListAsync();

            return new PagedResult2<RevenueEmployeeReportDisplay>(count, val.Offset, val.Limit)
            {
                Items = res
            };
        }

        public async Task<PagedResult2<RevenueReportDetailDisplay>> GetRevenueReportDetailPaged(RevenueReportDetailPaged val)
        {
            var query = await GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));

            var res = new List<RevenueReportDetailDisplay>();
            var count = 0;

            if (val.Date.HasValue)
            {
                query = query.Where(x => x.InvoiceDate.Value.Date == val.Date.Value.Date);
            }
            else if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            else if (val.EmployeeGroup)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }
            else if (val.AssistantGroup)
            {
                query = query.Where(x => x.AssistantId == val.AssistantId);
            }

            count = await query.CountAsync();
            query = query.OrderByDescending(x => x.InvoiceDate);
            if (val.Limit > 0) query = query.Skip(val.Offset).Take(val.Limit);
            res = await query.Select(x => new RevenueReportDetailDisplay
            {
                InvoiceDate = x.InvoiceDate,
                AssistantName = x.Assistant.Name,
                EmployeeName = x.Employee.Name,
                InvoiceOrigin = x.InvoiceOrigin,
                PartnerName = x.Partner.Name,
                PriceSubTotal = x.PriceSubTotal,
                ProductName = x.Product.Name
            }).ToListAsync();

            return new PagedResult2<RevenueReportDetailDisplay>(count, val.Offset, val.Limit)
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
