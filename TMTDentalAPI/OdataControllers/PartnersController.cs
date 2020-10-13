using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class PartnersController : BaseController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;

        public PartnersController(IPartnerService partnerService,
            IMapper mapper)
        {
            _partnerService = partnerService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _partnerService.GetViewModelsAsync();
            return Ok(results);
        }

       
    }
}
