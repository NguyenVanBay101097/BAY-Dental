﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IApplicationRoleFunctionService: IBaseService<ApplicationRoleFunction>
    {
        Task<ApplicationRoleFunctionHasAccessResult> HasAccess(IEnumerable<string> functions);
        Task<GetPermission> GetPermission();
        Task<GetPermission> GetPermission(string uid);
    }
}
