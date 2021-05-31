using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.ViewControllers
{
    public class PartnerAdvanceController : Controller
    {
        private readonly IPartnerAdvanceService _partnerAdvanceService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public PartnerAdvanceController(IPartnerAdvanceService partnerAdvanceService, IMapper mapper, IUserService userService)
        {
            _partnerAdvanceService = partnerAdvanceService;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var partnerAdvance = await _partnerAdvanceService.GetPartnerAdvancePrint(id);
            if (partnerAdvance == null)
                return NotFound();

            return View(partnerAdvance);
        }
    }
}
