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
    public class MedicineOrderController : Controller
    {
        private readonly IMedicineOrderService _medicineOrderService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public MedicineOrderController(IMedicineOrderService medicineOrderService, IViewToStringRenderService viewToStringRenderService)
        {
            _medicineOrderService = medicineOrderService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        //[CheckAccess(Actions = "Medicine.MedicineOrder.Read")]
        [PrinterNameFilterAttribute(Name = AppConstants.MedicineOrderPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {

            //var res = await _medicineOrderService.GetPrint(id);
            //if (res == null)
            //    return NotFound();

            //return View(res);
            return null;
        }
    }
}
