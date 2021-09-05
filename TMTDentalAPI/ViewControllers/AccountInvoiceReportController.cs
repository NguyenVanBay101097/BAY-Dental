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
            IMapper mapper)
        {
            _accountInvoiceReportService = accountInvoiceReportService;
            _viewToStringRenderService = viewToStringRenderService;
            _mapper = mapper;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.RevenueTimeReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenueTimeReport([FromBody] RevenueTimeReportPar val)
        {
            var data = await _accountInvoiceReportService.GetRevenueTimeReportPrint(val);
            return View(data);
        }

        [PrinterNameFilterAttribute(Name = AppConstants.RevenueServiceReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenueServiceReport([FromBody] RevenueServiceReportPar val)
        {
            var data = await _accountInvoiceReportService.GetRevenueServiceReportPrint(val);
            return View(data);
        }

        [PrinterNameFilterAttribute(Name = AppConstants.RevenueEmployeeReport)]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> PrintRevenueEmployeeReport([FromBody] RevenueEmployeeReportPar val)
        {
            var data = await _accountInvoiceReportService.GetRevenueEmployeeReportPrint(val);
            return View(data);
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
