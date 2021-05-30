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
    public class AccountPaymentsController : Controller
    {
        private readonly IAccountPaymentService _accountPaymentService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public AccountPaymentsController(IAccountPaymentService accountPaymentService, IViewToStringRenderService viewToStringRenderService)
        {
            _accountPaymentService = accountPaymentService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [CheckAccess(Actions = "Basic.AccountPayment.Read")]
        [PrinterNameFilterAttribute(Name = AppConstants.PaymentPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _accountPaymentService.GetPrint(id);
            if (res == null)
                NotFound();

            return View(res);
        }
    }
}
