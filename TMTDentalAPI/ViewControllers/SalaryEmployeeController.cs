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
    public class SalaryEmployeeController : Controller
    {
        private readonly IHrPayslipRunService _hrPayslipRunService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public SalaryEmployeeController(IHrPayslipRunService hrPayslipRunService, IViewToStringRenderService viewToStringRenderService)
        {
            _hrPayslipRunService = hrPayslipRunService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [HttpPut]
        [PrinterNameFilterAttribute(Name = AppConstants.SalaryPaymentPaperCode)]
        public async Task<IActionResult> Print(Guid id, [FromBody] HrPayslipRunSave val)
        {
            var ids = val.Slips.Where(x => x.IsCheck == true).Select(x => x.Id);
            await _hrPayslipRunService.UpdatePayslipRun(id, val);
            var res = await _hrPayslipRunService.GetHrPayslipRunForPrint(id);
            var viewdata = ViewData.ToDictionary(x => x.Key, x => x.Value);

            if (ids != null && ids.Any())
            {
                res.Slips = res.Slips.Where(x => ids.Contains(x.Id));
                var html = await _viewToStringRenderService.RenderViewAsync("SalaryEmployee/Print", res, viewdata);
                return Ok(new PrintData() { html = html });
            }
            else
            {
                return Ok(new PrintData() { html = null });
            }
        }
    }
}
