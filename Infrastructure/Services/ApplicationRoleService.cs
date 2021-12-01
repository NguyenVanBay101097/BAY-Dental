using ApplicationCore.Entities;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ApplicationRoleService : IApplicationRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationRoleService(RoleManager<ApplicationRole> roleManager, IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApplicationRole> CreateBaseUserRole()
        {
            var role = new ApplicationRole
            {
                Name = "BaseUser",
                Functions = new List<ApplicationRoleFunction>()
                {
                    new ApplicationRoleFunction { Func = "Basic.PartnerCategory.Read" },
                    new ApplicationRoleFunction { Func = "System.Company.Read"},
                    new ApplicationRoleFunction { Func = "Basic.Partner.Read"}
                },
                Hidden = true
            };

            await _roleManager.CreateAsync(role);
            return role;
        }

        public async Task CreateDefaultRoles()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\application_role.csv");
            if (!File.Exists(filePath))
                return;
            using (TextReader reader = File.OpenText(filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.BadDataFound = null;
                var roleCsvs = csv.GetRecords<ApplicationRoleCsvLine>().ToList();
                var roles = new List<ApplicationRole>();
                var irmodelDataObj = GetService<IIRModelDataService>();
                var listIrmodelDatas = new List<IRModelData>();
                foreach (var roleCsv in roleCsvs)
                {
                    var role = new ApplicationRole() { Name = roleCsv.Name };
                    if (await _roleManager.RoleExistsAsync(role.Name))
                        continue;
                    await _roleManager.CreateAsync(role);
                    roles.Add(role);
                    listIrmodelDatas.Add(new IRModelData()
                    {
                        Module = "base",
                        Model = "application.role",
                        ResId = role.Id,
                        Name = "application_role_" + roleCsv.Code
                    });
                }

                await irmodelDataObj.CreateAsync(listIrmodelDatas);
                await AddDefaultRoleFunction(roles.Select(x=> x.Id));
            }
        }

        public async Task AddDefaultRoleFunction(IEnumerable<string> RoleIds)
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\application_role_function.csv");
            if (!File.Exists(filePath))
                return;

            var irModelDataObj = GetService<IIRModelDataService>();
            var roleFuncObj = GetService<IApplicationRoleFunctionService>();
            var listIrModelDatas = await irModelDataObj.SearchQuery(x=> RoleIds.Contains(x.ResId)).ToListAsync();

            using (TextReader reader = File.OpenText(filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.BadDataFound = null;
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;

                var roleFuncCsvs = csv.GetRecords<ApplicationRoleFunctionCsvLine>().ToList();
                var roleFuncs = new List<ApplicationRoleFunction>();
                foreach (var irmodel in listIrModelDatas)
                {
                    foreach (var roleFunc in roleFuncCsvs)
                    {
                        var roleStrId = $"{irmodel.Module}.{irmodel.Name}";
                        if (roleStrId == roleFunc.RoleId.ToString())
                            roleFuncs.Add(new ApplicationRoleFunction()
                            {
                                RoleId = irmodel.ResId,
                                Func = roleFunc.Func
                            });
                    }
                }

                await roleFuncObj.CreateAsync(roleFuncs);
            }
        }

        private T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }
    }

    public class ApplicationRoleCsvLine
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class ApplicationRoleFunctionCsvLine
    {
        public string RoleId { get; set; }
        public string Func { get; set; }
    }
}
