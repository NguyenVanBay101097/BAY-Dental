using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class SaleOrderController : Controller
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;

        public SaleOrderController(ISaleOrderService saleOrderService, IViewToStringRenderService viewToStringRenderService,
            IMapper mapper,IUserService userService, ICompanyService companyService)
        {
            _saleOrderService = saleOrderService;
            _viewToStringRenderService = viewToStringRenderService;
            _mapper = mapper;
            _userService = userService;
            _companyService = companyService;
        }

        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        [PrinterNameFilterAttribute(Name = AppConstants.SaleOrderPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var saleOrder = await _saleOrderService.GetPrint(id);
            if (saleOrder == null)
                return NotFound();

            return View(saleOrder);

        }
        // in dự kiến doanh thu
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        [PrinterNameFilterAttribute(Name = AppConstants.SaleOrderPaperCode)]
        public async Task<IActionResult> PrintRevenueReport(SaleOrderRevenueReportPaged val)
        {
            var data = await _saleOrderService.GetRevenueReportPrint(val);
            return View(data);
        }

    }
}
