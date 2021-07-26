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
        public async Task<IActionResult> PrintRevenueReport(SaleOrderRevenueReportPrint val)
        {
            var query = _saleOrderService.SearchQuery(x => x.State != "cancel" && x.State != "draft");
            if (val.CompanyId.HasValue)
            {
                query = query.Where(x => x.CompanyId == val.CompanyId);
            }
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search) || x.Partner.Name.Contains(val.Search)
                                         || x.Partner.NameNoSign.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }


            var res = await _mapper.ProjectTo<SaleOrderRevenueReport>(query.Where(x => x.Residual > 0)
                .OrderByDescending(x => x.DateCreated))
                .ToListAsync();
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
            return View(res);

        }

    }
}
