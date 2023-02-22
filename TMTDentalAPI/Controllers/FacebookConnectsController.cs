using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookConnectsController : ControllerBase
    {
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly IFacebookPageService _facebookPageService;
        private readonly IMapper _mapper;

        public FacebookConnectsController(IFacebookConnectService facebookConnectService, IMapper mapper,
            IFacebookPageService facebookPageService)
        {
            _facebookConnectService = facebookConnectService;
            _facebookPageService = facebookPageService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var connect = await _facebookConnectService.SearchQuery().Include(x => x.Pages).FirstOrDefaultAsync();
            if (connect == null)
                return NoContent();
            var display = _mapper.Map<FacebookConnectDisplay>(connect);

            var connectedPages = await _facebookPageService.SearchQuery().ToListAsync();
            foreach (var item in display.Pages)
                item.Connected = connectedPages.Any(x => x.PageId == item.PageId);

            return Ok(display);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveFromUI(FacebookConnectSaveFromUI val)
        {
            await _facebookConnectService.SaveFromUI(val);
            return NoContent();
        }
    }
}