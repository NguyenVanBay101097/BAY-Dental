using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Caches;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ApplicationRoleFunctionService : BaseService<ApplicationRoleFunction>, IApplicationRoleFunctionService
    {
        private IMyCache _cache;
        private readonly AppTenant _tenant;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ApplicationRoleFunctionService(IAsyncRepository<ApplicationRoleFunction> repository, IHttpContextAccessor httpContextAccessor,
            IMyCache cache, ITenant<AppTenant> tenant, IWebHostEnvironment webHostEnvironment)
        : base(repository, httpContextAccessor)
        {
            _cache = cache;
            _tenant = tenant?.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<GetPermission> GetPermission()
        {
            var key = $"{(_tenant != null ? _tenant.Hostname : "localhost")}-permissions-{UserId}";

            var permission = await _cache.GetOrCreateAsync(key, async entry =>
            {
                var res = await SearchQuery(x => x.Role.UserRoles.Any(y => y.UserId == UserId)).Select(x => x.Func).ToListAsync();
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return res;
            });
            return new GetPermission() { IsUserRoot = IsUserRoot, Permission = permission };
        }

        public async Task<ApplicationRoleFunctionHasAccessResult> HasAccess(IEnumerable<string> functions)
        {
            if (IsUserRoot)
                return new ApplicationRoleFunctionHasAccessResult() { Access = true };

            //get list permission from DB
            var permissionList = (await GetPermission()).Permission;
            // tách functions to dict, example: dict["a"] = ["abc","abc.cde","abc.cde.efg"]
            var functionDics = new Dictionary<string, List<string>>();
            foreach (var func in functions)
            {
                functionDics.Add(func, new List<string>());
                var index = 0;
                if (func.Length <= 1) { functionDics[func].Add(func); continue; }
                while (index >= 0)
                {
                    index = func.IndexOf(".", index + 1);
                    if (index <= 0)
                    {
                        functionDics[func].Add(func); break;
                    }
                    var a = func.Substring(0, index);
                    functionDics[func].Add(a);
                }
            }
            //check, sử dụng ==, with all functions: any item in DB == any value of functionsDict => ok 
            var access = functionDics.All(x => permissionList.Any(s => x.Value.Any(xs => xs == s)));
            var errors = new List<string>();
            if (!access)
            {
                var functions_2 = functionDics.Where(x => !permissionList.Any(s => x.Value.Any(xs => xs == s)));
                var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\features.json");
                using (var reader = new StreamReader(filePath))
                {
                    var fileContent = reader.ReadToEnd();
                    var features = JsonConvert.DeserializeObject<List<PermissionTreeViewModel>>(fileContent);

                    foreach (var func in functions_2)
                    {
                        var function = features.SelectMany(x => x.Children).FirstOrDefault(x => func.Key.IndexOf(x.Permission) != -1);
                        if (function == null)
                            continue;

                        var op = function.Children.FirstOrDefault(x => x.Permission == func.Key);
                        var msg = $"Bạn không có quyền {(op != null ? op.Name.ToLower() : "xem")} {function.Name}";
                        errors.Add(msg);
                    }
                }
                if (errors.Count == 0) errors.Add("Bạn không có quyền thao tác!");
            }

            return new ApplicationRoleFunctionHasAccessResult() { Access = access, Error = string.Join(", ", errors) };
        }
    }

    public class ApplicationRoleFunctionHasAccessResult
    {
        public bool Access { get; set; }

        public string Error { get; set; }
    }

    public class GetPermission
    {
        public bool IsUserRoot { get; set; }
        public List<string> Permission { get; set; }
    }
}
