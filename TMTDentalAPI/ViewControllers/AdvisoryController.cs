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
        public async Task<IActionResult> Print(Guid customerId)
        {
            var res = new AdvisoryPrintVM();
            var partner = await _partnerService.SearchQuery(x => x.Id == customerId).Include(x => x.Company.Partner).FirstOrDefaultAsync();

            res.Partner = _mapper.Map<PartnerDisplay>(partner);
            res.Company = _mapper.Map<CompanyPrintVM>(partner.Company.Partner);

            var advisories = await _advisoryService.SearchQuery(x => x.CustomerId == customerId)
                .Include(x => x.AdvisoryProductRels).ThenInclude(x => x.Product)
                .Include(x => x.AdvisoryToothRels).ThenInclude(x => x.Tooth)
                .Include(x => x.User).ToListAsync();

            res.Advisories = _mapper.Map<IEnumerable<AdvisoryDisplay>>(advisories);

            res.User = _mapper.Map<ApplicationUserDisplay>(_userService.GetCurrentUser());
            return View(res);
        }
    }
}
