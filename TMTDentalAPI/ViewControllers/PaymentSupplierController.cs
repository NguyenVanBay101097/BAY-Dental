using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.ViewControllers
{
    public class PaymentSupplierController : Controller
    {
        private readonly IAccountPaymentService _accountPaymentService;

        public PaymentSupplierController(IAccountPaymentService accountPaymentService)
        {
            _accountPaymentService = accountPaymentService;
        }

        public async  Task<IActionResult> Print(Guid id)
        {
            var accountPayment = await _accountPaymentService.GetPrint(id);
            if (accountPayment == null)
                return NotFound();

            return View(accountPayment);
        }
    }
}
