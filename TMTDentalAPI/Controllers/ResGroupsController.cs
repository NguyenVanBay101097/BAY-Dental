using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
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
    public class ResGroupsController : BaseApiController
    {
        private readonly IResGroupService _resGroupService;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;

        public ResGroupsController(IResGroupService resGroupService,
            IMapper mapper, IIRModelAccessService modelAccessService)
        {
            _resGroupService = resGroupService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ResGroupPaged val)
        {
            _modelAccessService.Check("ResGroup", "Read");
            var result = await _resGroupService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _modelAccessService.Check("ResGroup", "Read");
            var group = await _resGroupService.SearchQuery(x => x.Id == id).Include(x => x.ModelAccesses)
                .Include("ModelAccesses.Model")
                .Include(x => x.ResGroupsUsersRels).Include("ResGroupsUsersRels.User").FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            var res = _mapper.Map<ResGroupDisplay>(group);
            res.ModelAccesses = res.ModelAccesses.OrderBy(x => x.Name);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResGroupDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ResGroup", "Create");
            var group = _mapper.Map<ResGroup>(val);
            SaveAccesses(val, group);
            SaveUsers(val, group);
            await _resGroupService.CreateAsync(group);

            val.Id = group.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResGroupDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ResGroup", "Write");
            var group = await _resGroupService.SearchQuery(x => x.Id == id).Include(x => x.ModelAccesses)
                .Include(x => x.ResGroupsUsersRels).FirstOrDefaultAsync();
            if (group == null)
                return NotFound();

            group = _mapper.Map(val, group);
            SaveAccesses(val, group);
            SaveUsers(val, group);
            await _resGroupService.UpdateAsync(group);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("ResGroup", "Unlink");
            var group = await _resGroupService.GetByIdAsync(id);
            if (group == null)
                return NotFound();
            await _resGroupService.DeleteAsync(group);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _resGroupService.DefaultGet();
            return Ok(res);
        }

        private void SaveAccesses(ResGroupDisplay val, ResGroup group)
        {
            foreach (var access in val.ModelAccesses)
            {
                if (access.Id == Guid.Empty)
                {
                    group.ModelAccesses.Add(_mapper.Map<IRModelAccess>(access));
                }
                else
                {
                    _mapper.Map(access, group.ModelAccesses.SingleOrDefault(c => c.Id == access.Id));
                }
            }
        }

        private void SaveUsers(ResGroupDisplay val, ResGroup group)
        {
            if (val.Users == null)
                return;
            group.ResGroupsUsersRels.Clear();
            foreach(var user in val.Users)
            {
                group.ResGroupsUsersRels.Add(new ResGroupsUsersRel
                {
                    UserId = user.Id
                });
            }
        }
    }
}