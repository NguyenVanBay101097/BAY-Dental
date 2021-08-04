using ApplicationCore.Constants;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class AccountInvoiceReportController : Controller
    {
        private readonly IAccountInvoiceReportService _accountInvoiceReportService;
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountInvoiceReportController(IAccountInvoiceReportService accountInvoiceReportService, 
            IViewToStringRenderService viewToStringRenderService,
            IUserService userService,
            ICompanyService companyService,
            IMapper mapper,
            CatalogDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _accountInvoiceReportService = accountInvoiceReportService;
            _viewToStringRenderService = viewToStringRenderService;
            _userService = userService;
            _companyService = companyService;
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

        [PrinterNameFilterAttribute(Name = AppConstants.RevenueTimeReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenueTimeReport([FromBody] RevenueTimeReportPar val)
        {
            var dict = new Dictionary<DateTime, RevenueTimeReportPrintVM>();
            var query1 = (GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId)))
                            .GroupBy(x => x.InvoiceDate.Value.Date);
            var query2 = (GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId)));
            
            var result1 = await query1.Select(x => new RevenueTimeReportPrintVM()
            {
                InvoiceDate = x.Key,
                PriceSubTotal = Math.Abs(x.Sum(s => s.PriceSubTotal))
            }).ToListAsync();

            foreach(var item in result1)
            {
                dict.Add(item.InvoiceDate.Date, item);
            }
            var result2 = await query2.Select(x => new RevenueReportDetailDisplay
            {
                InvoiceDate = x.InvoiceDate,
                AssistantName = x.Assistant.Name,
                PartnerName = x.Partner.Name,
                EmployeeName = x.Employee.Name,
                ProductName = x.Product.Name,
                InvoiceOrigin = x.InvoiceOrigin,
                PriceSubTotal = Math.Abs(x.PriceSubTotal)
            }).ToListAsync();

            foreach(var item in result2)
            {
                if (dict.ContainsKey(item.InvoiceDate.Value.Date))
                {
                    dict[item.InvoiceDate.Value.Date].RevenueReportDetailPrints
                     = dict[item.InvoiceDate.Value.Date].RevenueReportDetailPrints.Append(item).ToList();
                }
            }

            var user = await _userService.GetCurrentUser();
            ViewData["user"] = _mapper.Map<ApplicationUserSimple>(user);
            ViewData["company"] = await _companyService.SearchQuery(x => x.Id == user.CompanyId)
                .Include(x => x.Partner)
                .Select(x => new CompanyPrintVM
                {
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Logo = x.Logo,
                    PartnerCityName = x.Partner.CityName,
                    PartnerDistrictName = x.Partner.DisplayName,
                    PartnerWardName = x.Partner.WardName,
                    PartnerStreet = x.Partner.Street
                }).FirstOrDefaultAsync();

            ViewData["dateFrom"] = val.DateFrom.HasValue ? val.DateFrom : null;
            ViewData["dateTo"] = val.DateTo.HasValue ? val.DateTo : null;
            if (dict == null) return NotFound();
            
            return View(dict.Values.ToList().OrderByDescending(x => x.InvoiceDate));
        }

        [PrinterNameFilterAttribute(Name = AppConstants.RevenueServiceReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenueServiceReport([FromBody] RevenueServiceReportPar val)
        {
            var dict = new Dictionary<Guid, RevenueServiceReportPrintVM>();
            var query1 = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            if (val.ProductId.HasValue)
            {
                query1 = query1.Where(x => x.ProductId == val.ProductId);
            }
            var result1 = await query1.GroupBy(x => new { x.ProductId, x.Product.Name })
                .Select(x => new RevenueServiceReportPrintVM
                {
                    ProductId = x.Key.ProductId.Value,
                    ProductName = x.Key.Name,
                    PriceSubTotal = Math.Abs(x.Sum(s => s.PriceSubTotal))
                }).ToListAsync();
            var query2 = (GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId)));

            foreach (var item in result1)
            {
                dict.Add(item.ProductId, item);
            }
            var result2 = await query2.Select(
                
                x => new RevenueReportDetailDisplay
                {
                    ProductId = x.ProductId,
                    InvoiceDate = x.InvoiceDate,
                    AssistantName = x.Assistant.Name,
                    PartnerName = x.Partner.Name,
                    EmployeeName = x.Employee.Name,
                    ProductName = x.Product.Name,
                    InvoiceOrigin = x.InvoiceOrigin,
                    PriceSubTotal = Math.Abs(x.PriceSubTotal)
                }).ToListAsync();

            foreach (var item in result2)
            {
                if (dict.ContainsKey(item.ProductId.Value))
                {
                    dict[item.ProductId.Value].RevenueReportDetailPrints
                     = dict[item.ProductId.Value].RevenueReportDetailPrints.Append(item).ToList();
                }
            }

            var user = await _userService.GetCurrentUser();
            ViewData["user"] = _mapper.Map<ApplicationUserSimple>(user);
            ViewData["company"] = await _companyService.SearchQuery(x => x.Id == user.CompanyId)
                .Include(x => x.Partner)
                .Select(x => new CompanyPrintVM
                {
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Logo = x.Logo,
                    PartnerCityName = x.Partner.CityName,
                    PartnerDistrictName = x.Partner.DisplayName,
                    PartnerWardName = x.Partner.WardName,
                    PartnerStreet = x.Partner.Street
                }).FirstOrDefaultAsync();

            ViewData["dateFrom"] = val.DateFrom.HasValue ? val.DateFrom : null;
            ViewData["dateTo"] = val.DateTo.HasValue ? val.DateTo : null;
            if (dict == null) return NotFound();

            return View(dict.Values.ToList());
        }

        [PrinterNameFilterAttribute(Name = AppConstants.RevenueEmployeeReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenueEmployeeReport([FromBody] RevenueEmployeeReportPar val)
        {
            var dict = new Dictionary<Guid, RevenueEmployeeReportPrintVM>();
            var query1 = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            var query2 = GetRevenueReportQuery(new RevenueReportQueryCommon(val.DateFrom, val.DateTo, val.CompanyId));
            var result1 = new List<RevenueEmployeeReportPrintVM>();
            var result2 = new List<RevenueEmployeeAssistantReportDetailDisplay>();
            switch (val.GroupBy)
            {
                case "employee":
                    if (val.GroupById.HasValue)
                    {
                        query1 = query1.Where(x => x.EmployeeId == val.GroupById);
                        query2 = query2.Where(x => x.EmployeeId == val.GroupById);
                    }
                    result1 = await query1.GroupBy(x => new { x.EmployeeId, x.Employee.Name })
                        .Select(x => new RevenueEmployeeReportPrintVM
                        {
                            Id = x.Key.EmployeeId,
                            Name = x.Key.Name,
                            PriceSubTotal = Math.Abs(x.Sum(s => s.PriceSubTotal))
                        }).ToListAsync();
                    result2 = await query2.Select(
                            x => new RevenueEmployeeAssistantReportDetailDisplay
                            {
                                GroupById = x.EmployeeId,
                                ProductId = x.ProductId,
                                InvoiceDate = x.InvoiceDate,
                                AssistantName = x.Assistant.Name,
                                PartnerName = x.Partner.Name,
                                EmployeeName = x.Employee.Name,
                                ProductName = x.Product.Name,
                                InvoiceOrigin = x.InvoiceOrigin,
                                PriceSubTotal = Math.Abs(x.PriceSubTotal)
                            }).ToListAsync();
                    break;
                case "assistant":
                    if (val.GroupById.HasValue)
                    {
                        query1 = query1.Where(x => x.AssistantId == val.GroupById);
                        query2 = query2.Where(x => x.AssistantId == val.GroupById);
                    }
                    result1 = await query1.GroupBy(x => new { x.AssistantId, x.Assistant.Name })
                        .Select(x => new RevenueEmployeeReportPrintVM
                        {
                            Id = x.Key.AssistantId,
                            Name = x.Key.Name,
                            PriceSubTotal = Math.Abs(x.Sum(s => s.PriceSubTotal))
                        }).ToListAsync();
                    result2 = await query2.Select(
                            x => new RevenueEmployeeAssistantReportDetailDisplay
                            {
                                GroupById = x.AssistantId,
                                ProductId = x.ProductId,
                                InvoiceDate = x.InvoiceDate,
                                AssistantName = x.Assistant.Name,
                                PartnerName = x.Partner.Name,
                                EmployeeName = x.Employee.Name,
                                ProductName = x.Product.Name,
                                InvoiceOrigin = x.InvoiceOrigin,
                                PriceSubTotal = Math.Abs(x.PriceSubTotal)
                            }).ToListAsync();
                    break;
                default:
                    throw new Exception("Vui lòng chọn trường cần group");
            }

            foreach (var item in result1)
            {
                if (item.Id.HasValue)
                {
                    dict.Add(item.Id.Value, item);
                }
                else
                {
                    dict.Add(Guid.Empty, item);
                }
            }
          
            foreach (var item in result2)
            {
                if (item.GroupById.HasValue && dict.ContainsKey(item.GroupById.Value))
                {
                    dict[item.GroupById.Value].RevenueReportDetailPrints
                     = dict[item.GroupById.Value].RevenueReportDetailPrints.Append(item).ToList();
                }
                else
                {
                    dict[Guid.Empty].RevenueReportDetailPrints 
                        = dict[Guid.Empty].RevenueReportDetailPrints.Append(item).ToList();
                }
            }

            var user = await _userService.GetCurrentUser();
            ViewData["user"] = _mapper.Map<ApplicationUserSimple>(user);
            ViewData["company"] = await _companyService.SearchQuery(x => x.Id == user.CompanyId)
                .Include(x => x.Partner)
                .Select(x => new CompanyPrintVM
                {
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Logo = x.Logo,
                    PartnerCityName = x.Partner.CityName,
                    PartnerDistrictName = x.Partner.DisplayName,
                    PartnerWardName = x.Partner.WardName,
                    PartnerStreet = x.Partner.Street
                }).FirstOrDefaultAsync();

            ViewData["dateFrom"] = val.DateFrom.HasValue ? val.DateFrom : null;
            ViewData["dateTo"] = val.DateTo.HasValue ? val.DateTo : null;
            if (dict == null) return NotFound();

            return View(dict.Values.ToList());
        }

        [PrinterNameFilterAttribute(Name = AppConstants.RevenuePartnerReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenuePartnerReport([FromBody] RevenuePartnerReportPar val)
        {
            var data = await _accountInvoiceReportService.GetRevenuePartnerReportPrint(val);
            return View(data);
        } 

    }
}
