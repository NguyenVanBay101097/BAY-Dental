using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class AdvisoryController : Controller
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IPartnerService _partnerService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public AdvisoryController(IAdvisoryService advisoryService, IPartnerService partnerService, IMapper mapper, IUserService userService)
        {
            _partnerService = partnerService;
            _advisoryService = advisoryService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Print(IEnumerable<Guid> ids)
        {
            var res = await _advisoryService.Print(ids);
            if (res == null) return NotFound();

            return View(res);
        }
    }
}
