using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
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
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ResGroupsController(IResGroupService resGroupService,
            IMapper mapper, IUserService userService, IUnitOfWorkAsync unitOfWork)
        {
            _resGroupService = resGroupService;
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ResGroupPaged val)
        {
            var result = await _resGroupService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var group = await _resGroupService.SearchQuery(x => x.Id == id).Include(x => x.ModelAccesses)
                .Include("ModelAccesses.Model")
                .Include(x => x.ResGroupsUsersRels).Include("ResGroupsUsersRels.User").FirstOrDefaultAsync();
            if (group == null)
                return NotFound();

            var res = _mapper.Map<ResGroupDisplay>(group);
            res.ModelAccesses = res.ModelAccesses.OrderBy(x => x.Name);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResGroupDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();

            var group = _mapper.Map<ResGroup>(val);
            SaveAccesses(val, group);

            await _resGroupService.CreateAsync(group);

            _unitOfWork.Commit();

            val.Id = group.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResGroupDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var group = await _resGroupService.SearchQuery(x => x.Id == id).Include(x => x.ModelAccesses)
                .Include(x => x.ResGroupsUsersRels).FirstOrDefaultAsync();

            if (group == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            group = _mapper.Map(val, group);
            SaveAccesses(val, group);

            var refreshCacheUserIds = new List<string>().AsEnumerable();
            refreshCacheUserIds = refreshCacheUserIds.Union(group.ResGroupsUsersRels.Select(x => x.UserId).ToList());
            refreshCacheUserIds = refreshCacheUserIds.Union(val.Users.Select(x => x.Id).ToList());

            await _resGroupService.UpdateAsync(group);

            _userService.ClearSecurityCache(refreshCacheUserIds);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
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
        public async Task<IActionResult> ResetSecurityData()
        {
            await _unitOfWork.BeginTransactionAsync();
            await _resGroupService.ResetSecurityData();
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateModels()
        {
            await _unitOfWork.BeginTransactionAsync();
            await _resGroupService.UpdateModels();
            _unitOfWork.Commit();
            return NoContent();
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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetByModelDataModuleName(ResGroupByModulePar val)
        {
            var res = await _resGroupService.GetByModelDataModuleName(val);
            return Ok(res);
        }
    }
}