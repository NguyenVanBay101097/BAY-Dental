using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
        private readonly IMyCache _cache;
        private readonly IUserService _userService;

        public ResGroupsController(IResGroupService resGroupService,
            IMapper mapper, IIRModelAccessService modelAccessService,
            IMyCache cache, IUserService userService)
        {
            _resGroupService = resGroupService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
            _cache = cache;
            _userService = userService;
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

            var refreshCacheUserIds = new List<string>().AsEnumerable();
            refreshCacheUserIds = refreshCacheUserIds.Union(group.ResGroupsUsersRels.Select(x => x.UserId).ToList());
            refreshCacheUserIds = refreshCacheUserIds.Union(val.Users.Select(x => x.Id).ToList());

            SaveUsers(val, group);
            await _resGroupService.CreateAsync(group);

            _userService.ClearSecurityCache(refreshCacheUserIds);

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

            var refreshCacheUserIds = new List<string>().AsEnumerable();
            refreshCacheUserIds = refreshCacheUserIds.Union(group.ResGroupsUsersRels.Select(x => x.UserId).ToList());
            refreshCacheUserIds = refreshCacheUserIds.Union(val.Users.Select(x => x.Id).ToList());

            SaveUsers(val, group);
            await _resGroupService.UpdateAsync(group);

            _userService.ClearSecurityCache(refreshCacheUserIds);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("ResGroup", "Unlink");
            var group = await _resGroupService.SearchQuery(x => x.Id == id).Include(x => x.ModelAccesses)
               .Include(x => x.ResGroupsUsersRels).FirstOrDefaultAsync();
            if (group == null)
                return NotFound();
            await _resGroupService.DeleteAsync(group);

            var refreshCacheUserIds = new List<string>().AsEnumerable();
            refreshCacheUserIds = refreshCacheUserIds.Union(group.ResGroupsUsersRels.Select(x => x.UserId).ToList());
            _userService.ClearSecurityCache(refreshCacheUserIds);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _resGroupService.DefaultGet();
            return Ok(res);
        }

        [HttpPost("[action]")]
        public IActionResult ClearCache()
        {
            _modelAccessService.Check("ResGroup", "Write");
            _cache.Clear();
            return Ok(true);
        }

        private void SaveAccesses(ResGroupDisplay val, ResGroup group)
        {
            var existAccesses = group.ModelAccesses.ToList();
            var lineToRemoves = new List<IRModelAccess>();
            foreach (var existLine in existAccesses)
            {
                bool found = false;
                foreach (var item in val.ModelAccesses)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                group.ModelAccesses.Remove(line);
            }

            foreach (var line in val.ModelAccesses)
            {
                if (line.Id == Guid.Empty)
                {
                    var acccess = _mapper.Map<IRModelAccess>(line);
                    group.ModelAccesses.Add(acccess);
                }
                else
                {
                    var acccess = group.ModelAccesses.SingleOrDefault(c => c.Id == line.Id);
                    if (acccess != null)
                    {
                        _mapper.Map(line, acccess);
                    }
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