using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class ToaThuocController : Controller
    {
        private readonly IToaThuocService _toaThuocService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public ToaThuocController(IToaThuocService toaThuocService,IViewToStringRenderService viewToStringRenderService)
        {
            _toaThuocService = toaThuocService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.ToaThuocPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _toaThuocService.GetToaThuocPrint(id);
            if (res == null)
                return NotFound();

            return View(res);
        }
    }
}
