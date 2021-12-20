﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Volo.Abp.Security.Claims
{
    public interface IAbpClaimsPrincipalFactory
    {
        Task<ClaimsPrincipal> CreateAsync(ClaimsPrincipal existsClaimsPrincipal = null);
    }
}
