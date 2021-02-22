using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class SalaryEmployeeController : Controller
    {
        private readonly IHrPayslipRunService _hrPayslipRunService;

        public SalaryEmployeeController(IHrPayslipRunService hrPayslipRunService)
        {
            _hrPayslipRunService = hrPayslipRunService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _hrPayslipRunService.GetHrPayslipRunForDisplay(id);
            if (res == null)
                return NotFound();

            return View(res);
        }
    }
}
