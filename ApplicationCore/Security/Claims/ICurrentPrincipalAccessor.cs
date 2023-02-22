﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Volo.Abp.Security.Claims
{
    public interface ICurrentPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }

        IDisposable Change(ClaimsPrincipal principal);
    }
}
