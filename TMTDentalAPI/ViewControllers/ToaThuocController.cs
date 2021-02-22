using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.ViewControllers
{
    public class ToaThuocController : Controller
    {
        private readonly IToaThuocService _toaThuocService;

        public ToaThuocController(IToaThuocService toaThuocService)
        {
            _toaThuocService = toaThuocService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _toaThuocService.GetToaThuocPrint(id);
            return View(res);
        }
    }
}
