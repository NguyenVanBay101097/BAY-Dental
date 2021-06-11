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
    public class AccountInvoiceReportService : IAccountInvoiceReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountInvoiceReportService(CatalogDbContext context, IAsyncRepository<Product> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }
        private IQueryable<AccountInvoiceReport> GetRevenueReportQuery(RevenueReportQueryCommon val)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();

            var query = _context.AccountInvoiceReports.Where(x => (x.Type == "out_invoice" || x.Type == "out_refund") && x.State == "posted"
            && x.AccountInternalType != "receivable"
            && (!x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value))
            ).AsQueryable();

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

        public async Task<IEnumerable<RevenueTimeReportDisplay>> GetRevenueTimeReport(RevenueTimeReportPar val)
        {
            var query = (GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId)))
                            .GroupBy(x => x.InvoiceDate.Value.Date);

            var res = await query.Select(x => new RevenueTimeReportDisplay
            {
                InvoiceDate = x.Key as DateTime?,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo   = val.DateTo
            }).ToListAsync();

            res = res.OrderByDescending(z => z.InvoiceDate.Value).ToList();

            return res;
        }

        public async Task<IEnumerable<RevenueServiceReportDisplay>> GetRevenueServiceReport(RevenueServiceReportPar val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            var queryRes = query.GroupBy(x => new { x.ProductId, x.Product.Name });

            var res = await queryRes.Select(x => new RevenueServiceReportDisplay
            {
                ProductId = x.Key.ProductId.Value,
                ProductName = x.Key.Name,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                DateTo = val.DateTo,
                DateFrom = val.DateFrom,
                CompanyId = val.CompanyId
            }).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<RevenueEmployeeReportDisplay>> GetRevenueEmployeeReport(RevenueEmployeeReportPar val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            IQueryable<IGrouping<object, AccountInvoiceReport>> queryRes;

            switch (val.GroupBy)
            {
                case"employee":
                    if (val.GroupById.HasValue)
                    {
                        query = query.Where(x => x.EmployeeId == val.GroupById);
                    }
                    queryRes = query.GroupBy(x => new EmployeeAssistantKeyGroup { Id = x.EmployeeId, Name = x.Employee.Name });
                    break;
                case "assistant":
                    if (val.GroupById.HasValue)
                    {
                        query = query.Where(x => x.AssistantId == val.GroupById);
                    }
                    queryRes = query.GroupBy(x => new EmployeeAssistantKeyGroup { Id = x.AssistantId, Name = x.Assistant.Name });
                    break;
                default:
                    throw new Exception("Vui lòng chọn trường cần group");
            }
           
            var res = await queryRes.Select(x => new RevenueEmployeeReportDisplay
            {
                EmployeeId = (x.Key as EmployeeAssistantKeyGroup).Id,
                EmployeeName = (x.Key as EmployeeAssistantKeyGroup).Name,
                PriceSubTotal = Math.Abs(x.Sum(z => z.PriceSubTotal)),
                GroupBy = val.GroupBy,
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                ToDetailEmployeeId = (x.Key as EmployeeAssistantKeyGroup).Id ?? Guid.Empty
            }).ToListAsync();

            return res;
        }

        public async Task<PagedResult2<RevenueReportDetailDisplay>> GetRevenueReportDetailPaged(RevenueReportDetailPaged val)
        {
            var query = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));

            var res = new List<RevenueReportDetailDisplay>();
            var count = 0;

            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }
            if (val.EmployeeId.HasValue && val.EmployeeId != Guid.Empty)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }
            if (val.EmployeeId.HasValue && val.EmployeeId == Guid.Empty)
            {
                query = query.Where(x => x.EmployeeId == null);
            }
            if (val.AssistantId.HasValue && val.AssistantId != Guid.Empty)
            {
                query = query.Where(x => x.AssistantId == val.AssistantId);
            }
            if (val.AssistantId.HasValue && val.AssistantId == Guid.Empty)
            {
                query = query.Where(x => x.AssistantId == null);
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
                PriceSubTotal = Math.Abs(x.PriceSubTotal),
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
