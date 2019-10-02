using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    public class AccountInvoiceReportService: BaseService<Product>, IAccountInvoiceReportService
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

        public async Task<IEnumerable<AccountInvoiceReportByTimeItem>> GetSummaryByTime(AccountInvoiceReportByTimeSearch val)
        {
            var today = DateTime.Today;
            var states = new string[] { "open", "paid" };
            //xác định ngày mặc định nếu ko nhập cho từng trường hợp group by
            if (val.GroupBy == "year")
            {
                var dateFrom = val.YearFrom.HasValue ? new DateTime(val.YearFrom.Value.Year, 1, 1) : new DateTime(today.Year, 1, 1);
                var dateTo = val.YearTo.HasValue ? new DateTime(val.YearTo.Value.Year + 1, 1, 1).AddMinutes(-1) : new DateTime(today.Year + 1, 1, 1).AddMinutes(-1);
                var result = await _context.AccountInvoiceReports.Where(x => x.date >= dateFrom && x.date <= dateTo && states.Contains(x.state))
                    .GroupBy(x => x.date.Value.Year)
                    .Select(x => new AccountInvoiceReportByTimeItem
                    {
                        Date = new DateTime(x.Key, 1, 1),
                        AmountTotal = x.Sum(s => s.price_total - s.discount_amount),
                        Residual = x.Sum(s => s.residual),
                    }).ToListAsync();
                foreach(var item in result)
                {
                    item.DateStr = item.Date.Value.ToString("yyyy");
                    item.DateFrom = item.Date;
                    item.DateTo = item.Date.Value.AddYears(1).AddMinutes(-1);
                }
                return result;
            }
            else if (val.GroupBy == "month")
            {
                var dateFrom = val.MonthFrom.HasValue ? new DateTime(val.MonthFrom.Value.Year, val.MonthFrom.Value.Month, 1)
                    : new DateTime(today.Year, today.Month, 1);
                var dateTo = val.MonthTo.HasValue ? new DateTime(val.MonthTo.Value.Year, val.MonthTo.Value.Month + 1, 1).AddMinutes(-1)
                    : new DateTime(today.Year, today.Month + 1, 1).AddMinutes(-1);
                var result = await _context.AccountInvoiceReports.Where(x => x.date >= dateFrom && x.date <= dateTo && states.Contains(x.state))
                    .GroupBy(x => new {
                        x.date.Value.Year,
                        x.date.Value.Month,
                    })
                    .Select(x => new AccountInvoiceReportByTimeItem
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, 1),
                        AmountTotal = x.Sum(s => s.price_total - s.discount_amount),
                        Residual = x.Sum(s => s.residual),
                    }).ToListAsync();
                foreach (var item in result)
                {
                    item.DateStr = item.Date.Value.ToString("MM/yyyy");
                    item.DateFrom = item.Date;
                    item.DateTo = item.Date.Value.AddMonths(1).AddMinutes(-1);
                }
                return result;
            }
            else //default day
            {
                var dateFrom = val.DateFrom.HasValue ? val.DateFrom : today;
                var dateTo = val.DateTo.HasValue ? val.DateTo.Value.AddDays(1).AddMinutes(-1)
                    : today.AddDays(1).AddMinutes(-1);
                var result = await _context.AccountInvoiceReports.Where(x => x.date >= dateFrom && x.date <= dateTo && states.Contains(x.state))
                    .GroupBy(x => new {
                        x.date.Value.Year,
                        x.date.Value.Month,
                        x.date.Value.Day,
                    })
                    .Select(x => new AccountInvoiceReportByTimeItem
                    {
                        Date = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                        AmountTotal = x.Sum(s => s.price_total - s.discount_amount),
                        Residual = x.Sum(s => s.residual),
                    }).ToListAsync();
                foreach (var item in result)
                {
                    item.DateStr = item.Date.Value.ToString("dd/MM/yyyy");
                    item.DateFrom = item.Date;
                    item.DateTo = item.Date.Value.AddDays(1).AddMinutes(-1);
                }
                return result;
            }
        }

        public async Task<IEnumerable<AccountInvoiceReportByTimeDetail>> GetDetailByTime(AccountInvoiceReportByTimeItem val)
        {
            var today = DateTime.Today;
            var states = new string[] { "open", "paid" };
            var dateFrom = val.DateFrom;
            var dateTo = val.DateTo;
            var result = await _context.AccountInvoiceReports.Where(x => x.date >= dateFrom && x.date <= dateTo && states.Contains(x.state))
                .GroupBy(x => new {
                    InvoiceId = x.invoice_id,
                    Number = x.number,
                    DateInvoice = x.date
                })
                .Select(x => new AccountInvoiceReportByTimeDetail
                {
                    Date = x.Key.DateInvoice,
                    InvoiceId = x.Key.InvoiceId,
                    Number = x.Key.Number,
                    AmountTotal = x.Sum(s => s.price_total - s.discount_amount),
                    Residual = x.Sum(s => s.residual),
                }).ToListAsync();
            
            return result;
        }

        public async Task<AccountInvoiceReportHomeSummaryVM> GetToDaySummary()
        {
            var dateFrom = DateTime.Today;
            var dateTo = dateFrom.AddDays(1);
            var states = new string[] { "open", "paid" };
            var query = _context.AccountInvoiceReports.Where(x => x.date >= dateFrom && x.date < dateTo && states.Contains(x.state));
            var totalInvoice = await query.GroupBy(x => x.invoice_id).CountAsync();
            var totalAmount = await query.SumAsync(x => x.amount_total);
            return new AccountInvoiceReportHomeSummaryVM
            {
                TotalAmount = totalAmount,
                TotalInvoice = totalInvoice
            };
        }

        public async Task<IEnumerable<AccountInvoiceReportAmountResidual>> GetAmountResidualToday()
        {
            var dateFrom = DateTime.Today;
            var dateTo = dateFrom.AddDays(1);
            var states = new string[] { "open", "paid" };
            var query = _context.AccountInvoiceReports.Where(x => x.date >= dateFrom && x.date < dateTo && states.Contains(x.state));
            var totalAmount = await query.SumAsync(x => x.price_total - x.discount_amount);
            var totalResidual = await query.SumAsync(x => x.residual);

            var list = new List<AccountInvoiceReportAmountResidual>();
            list.Add(new AccountInvoiceReportAmountResidual { Name = "AmountTotal", Value = totalAmount });
            list.Add(new AccountInvoiceReportAmountResidual { Name = "Residual", Value = totalResidual });
            return list;
        }

        public async Task<IEnumerable<AccountInvoiceReportTopServices>> GetTopServices(int number)
        {
            var prdObj = GetService<IProductService>();
            var query = await _context.AccountInvoiceReports
                .GroupBy(x => new
                {
                    ProductId = x.product_id,
                    ProductName = x.Product.Name
                })
                .Select(x => new AccountInvoiceReportTopServices
                {
                    ProductId = x.Key.ProductId,
                    ProductQtyTotal = x.Sum(y => y.product_qty),
                    ProductName = x.Key.ProductName
                })
                .OrderByDescending(x => x.ProductQtyTotal).Take(number)
                .ToListAsync();

            return query;
        }
    }
}
