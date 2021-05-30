using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ApplicationRoleService: IApplicationRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public ApplicationRoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<ApplicationRole> CreateBaseUserRole()
        {
            var role = new ApplicationRole
            {
                Name = "BaseUser",
                Functions = new List<ApplicationRoleFunction>()
                {
                    new ApplicationRoleFunction { Func = "Basic.PartnerCategory.Read" },
                },
                Hidden = true
            };

            await _roleManager.CreateAsync(role);
            return role;
        }
    }
}
