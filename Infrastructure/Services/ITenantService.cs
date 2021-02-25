﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITenantService : IAdminBaseService<AppTenant>
    {
        Task<PagedResult2<TenantBasic>> GetPagedResultAsync(TenantPaged val);
        Task CheckUpdateDateExpired(TenantUpdateDateExpiredViewModel val);
    }
}
