using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
    public class FacebookTagsController : BaseApiController
    {
        private readonly IFacebookTagService _facebookTagService;
        private readonly IMapper _mapper;

        public FacebookTagsController(IFacebookTagService facebookTagService, IMapper mapper)
        {
            _facebookTagService = facebookTagService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SimplePaged val)
        {
            var res = await _facebookTagService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var tag = await _facebookTagService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (tag == null)
                return NotFound();
            return Ok(_mapper.Map<FacebookTagBasic>(tag));
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacebookTagSave val)
        {
            var tag = _mapper.Map<FacebookTag>(val);
            await _facebookTagService.CreateAsync(tag);

            var basic = _mapper.Map<FacebookTagBasic>(tag);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FacebookTagSave val)
        {
            var tag = await _facebookTagService.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            tag = _mapper.Map(val, tag);
            await _facebookTagService.UpdateAsync(tag);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var tag = await _facebookTagService.GetByIdAsync(id);
            if (tag == null)
                return NotFound();
            await _facebookTagService.DeleteAsync(tag);

            return NoContent();
        }
    }
}