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
    public class PartnerOldNewReportController : Controller
    {
        private readonly IPartnerOldNewReportService _partnerOldNewReportService;
        private readonly IViewToStringRenderService _viewToStringRenderService;


        public PartnerOldNewReportController(IPartnerOldNewReportService partnerOldNewReportService, IViewToStringRenderService viewToStringRenderService)
        {
            _partnerOldNewReportService = partnerOldNewReportService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.PartnerOldNewReport)]
        public async Task<IActionResult> GetReportPrint(PartnerOldNewReportReq val)
        {
            var res = await _partnerOldNewReportService.GetReportPrint(val);
            return View(res);
        }

    }
}
