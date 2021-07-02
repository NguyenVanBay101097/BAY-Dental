using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class AdvisoryController : Controller
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        public AdvisoryController(IAdvisoryService advisoryService, IPartnerService partnerService, IMapper mapper)
        {
            _partnerService = partnerService;
            _advisoryService = advisoryService;
            _mapper = mapper;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.AdvisoryPaperCode)]
        public async Task<IActionResult> Print(IEnumerable<Guid> ids)
        {
            var res = await _advisoryService.Print(ids);
            if (res == null) return NotFound();

            return View(res);
        }
    }
}
