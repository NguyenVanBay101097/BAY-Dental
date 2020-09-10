using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ApplicationRoleFunctionService : BaseService<ApplicationRoleFunction>, IApplicationRoleFunctionService
    {
        private IMyCache _cache;
        private readonly AppTenant _tenant;
        public ApplicationRoleFunctionService(IAsyncRepository<ApplicationRoleFunction> repository, IHttpContextAccessor httpContextAccessor,
            IMyCache cache, ITenant<AppTenant> tenant)
        : base(repository, httpContextAccessor)
        {
            _cache = cache;
            _tenant = tenant?.Value;
        }

        public async Task<bool> HasAccess(List<string> functions, bool raiseException = true)
        {
            if (string.IsNullOrEmpty(UserId))
                return false;
            var key = (_tenant != null ? _tenant.Hostname : "localhost") + $@"-permission-{UserId}";

            //get list permission
            var permissionList = await _cache.GetOrCreateAsync(key, async entry =>
            {
                var res = await SearchQuery(x => x.Role.UserRoles.Any(y => y.UserId == UserId)).Select(x => x.Func).ToListAsync();
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return res;
            });

            //check
            return functions.All(x => permissionList.Any(s => x.IndexOf(s) != -1));
        }
    }
}
