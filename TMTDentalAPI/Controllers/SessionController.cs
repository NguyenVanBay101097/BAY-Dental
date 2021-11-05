using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Session;

namespace TMTDentalAPI.Controllers
{
    [Route("Web/Session")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly AppTenant _tenant;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IUserService _userService;

        public SessionController(ITenant<AppTenant> tenant,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IApplicationRoleFunctionService roleFunctionService,
            IUserService userService)
        {
            _tenant = tenant?.Value;
            _userManager = userManager;
            _mapper = mapper;
            _roleFunctionService = roleFunctionService;
            _userService = userService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSessionInfo()
        {
            var user = await _userManager.Users.Where(x => x.UserName == User.Identity.Name)
                .Include(x => x.Company)
                .Include(x => x.ResCompanyUsersRels).ThenInclude(x => x.Company)
                .FirstOrDefaultAsync();

            var getPermissionResult = await _roleFunctionService.GetPermission(user.Id);
            var groups = await _userService.GetGroups(user.Id);
            var sessionInfo = new SessionUserInfo
            {
                Name = User.Identity.Name,
                ExpirationDate = _tenant?.DateExpired,
                IsAdmin = user.IsUserRoot,
                Permissions = getPermissionResult.Permission,
                Groups = groups,
                UserCompanies = new SessionUserCompany
                {
                    CurrentCompany = _mapper.Map<CompanySimple>(user.Company),
                    AllowedCompanies = user.ResCompanyUsersRels.Select(x => _mapper.Map<CompanySimple>(x.Company))
                }
            };

            return Ok(sessionInfo);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var user = await _userManager.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefaultAsync();
            return Ok(_mapper.Map<UserInfoDto>(user));
        }
    }
}
