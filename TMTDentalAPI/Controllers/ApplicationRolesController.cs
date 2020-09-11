using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationRolesController : BaseApiController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPartnerService _partnerService;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IMyCache _cache;

        public ApplicationRolesController(RoleManager<ApplicationRole> roleManager,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IPartnerService partnerService,
            UserManager<ApplicationUser> userManager,
            IApplicationRoleFunctionService roleFunctionService,
            IMyCache cache)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerService = partnerService;
            _userManager = userManager;
            _roleFunctionService = roleFunctionService;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ApplicationRolePaged val)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.NormalizedName.Contains(val.Search));
            query = query.OrderBy(x => x.Name);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            var result = new PagedResult2<ApplicationRoleBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ApplicationRoleBasic>>(items)
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var role = await _roleManager.Roles.Where(x => x.Id == id).Include(x => x.Functions).FirstOrDefaultAsync();
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            if (role == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<ApplicationRoleDisplay>(role);
            res.Users = _mapper.Map<IEnumerable<ApplicationUserSimple>>(users);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRoleSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var role = _mapper.Map<ApplicationRole>(val);
           
            foreach(var function in val.Functions)
            {
                role.Functions.Add(new ApplicationRoleFunction()
                {
                    Func = function
                });
            }

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors.Select(x => x.Description)));
            }

            foreach (var userId in val.UserIds)
            {
                var usr = await _userManager.FindByIdAsync(userId);
                var result2 = await _userManager.AddToRoleAsync(usr, role.Name);
                if (!result2.Succeeded)
                {
                    throw new Exception(string.Join(";", result2.Errors.Select(x => x.Description)));
                }
            }

            ClearPermissionsCache(val.UserIds);

            _unitOfWork.Commit();

            var basic = _mapper.Map<ApplicationRoleBasic>(role);
            return Ok(basic);
        }

        private void ClearPermissionsCache(IEnumerable<string> userIds)
        {
            foreach(var userId in userIds) 
                _cache.RemoveByPattern($"permissions-{userId}");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ApplicationRoleSave val)
        {
            var role = await _roleManager.Roles.Where(x => x.Id == id).Include(x => x.Functions).FirstOrDefaultAsync();
            if (role == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            role = _mapper.Map(val, role);
            role.Functions.Clear();
            foreach (var function in val.Functions)
            {
                role.Functions.Add(new ApplicationRoleFunction()
                {
                    Func = function
                });
            }

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors.Select(x => x.Description)));
            }

            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            var toRemove = users.Where(x => !val.UserIds.Any(s => s == x.Id)).ToList();
            var toRemoveIds = toRemove.Select(x => x.Id).ToList();
            foreach (var u in toRemove)
            {
                var result2 = await _userManager.RemoveFromRoleAsync(u, role.Name);
                if (!result2.Succeeded)
                {
                    throw new Exception(string.Join(";", result2.Errors.Select(x => x.Description)));
                }
            }

            var toAdd = val.UserIds.Where(x => !users.Any(s => s.Id == x)).ToList();

            foreach (var userId in toAdd)
            {
                var usr = await _userManager.FindByIdAsync(userId);
                var result2 = await _userManager.AddToRoleAsync(usr, role.Name);
                if (!result2.Succeeded)
                {
                    throw new Exception(string.Join(";", result2.Errors.Select(x => x.Description)));
                }
            }

            //clear cache cho những user remove khỏi nhóm và thuộc nhóm này
            ClearPermissionsCache(toRemoveIds.Union(val.UserIds));

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            var user = await _roleManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var result = await _roleManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors.Select(x => x.Description)));
            }
            return NoContent();
        }
    }
}