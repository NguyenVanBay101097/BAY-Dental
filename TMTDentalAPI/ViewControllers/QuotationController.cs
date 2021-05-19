using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.ViewControllers
{
    public class QuotationController : Controller
    {
        private readonly IQuotationService _quotationService;
        public QuotationController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _quotationService.Print(id);
            if (res == null) return NotFound();

            return View(res);
        }
    }
}
