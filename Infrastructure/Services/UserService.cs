using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Dapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CatalogDbContext _dbContext;
        private readonly IMyCache _cache;

        public UserService(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IMyCache cache, CatalogDbContext dbContext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _dbContext = dbContext;
        }

        protected string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
            var userId = UserId;
            if (string.IsNullOrEmpty(userId))
                return null;
            return await _userManager.FindByIdAsync(userId);
        }

        public IQueryable<ApplicationUser> GetQueryById(string id)
        {
            return _userManager.Users.Where(x => x.Id == id);
        }

        //public override ISpecification<ApplicationUser> RuleDomainGet(IRRule rule)
        //{
        //    var companyId = CompanyId;
        //    switch (rule.Code)
        //    {
        //        case "account.invoice_comp_rule":
        //            return new InitialSpecification<ApplicationUser>(x => x.CompanyId == companyId);
        //        default:
        //            return null;
        //    }
        //}

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public async Task<List<string>> GetGroups()
        {
            var userId = UserId;
            var groupIds = await _userManager.Users.Where(x => x.Id == userId).SelectMany(x => x.ResGroupsUsersRels)
                .Select(x => x.GroupId.ToString()).ToListAsync();
            var irModelDataObj = GetService<IIRModelDataService>();
            var res = await irModelDataObj.SearchQuery(x => x.Model == "res.groups" && groupIds.Contains(x.ResId))
                .Select(x => x.Module + "." + x.Name).ToListAsync();
            return res;
        }

        public void ClearSecurityCache(IEnumerable<string> ids)
        {
            var tenant = _httpContextAccessor.HttpContext.GetTenant<AppTenant>();
            foreach (var id in ids)
            {
                _cache.RemoveByPattern(string.Format("{0}ir.model.access-{1}", tenant != null ? tenant.Hostname + "-" : "", id));
                _cache.RemoveByPattern(string.Format("{0}ir.rule-{1}", tenant != null ? tenant.Hostname + "-" : "", id));
            }
        }

        public void ClearRuleCache(IEnumerable<string> ids)
        {
            var tenant = _httpContextAccessor.HttpContext.GetTenant<AppTenant>();
            foreach (var id in ids)
            {
                _cache.RemoveByPattern(string.Format("{0}ir.rule-{1}", tenant != null ? tenant.Hostname + "-" : "", id));
            }
        }

        public async Task<bool> HasGroup(string group_ext_id)
        {
            var uid = UserId;
            var tmp = group_ext_id.Split(".");
            var module = tmp[0];
            var name = tmp[1];
            var result = await _dbContext.ResGroupsUsersRels.FromSqlRaw("SELECT * FROM ResGroupsUsersRels " +
                                        "WHERE UserId = @p0 and GroupId in (SELECT ResId FROM IRModelDatas WHERE Module = @p1 and Name = @p2)", uid, module, name).ToListAsync();
            return result.Count > 0;
        }

        public void TestJobFunc(string s, string tenant_id)
        {
            using (var conn = new SqlConnection($"Server=.\\SQLEXPRESS;User Id=sa;Password=123123;Initial Catalog=TMTDentalCatalogDb__{tenant_id};"))
            {
                try
                {
                    conn.Open();
                    var orderIds = conn.Query<int>(
                        @"SELECT COUNT(*) from Partners");
                }
                catch (SqlException exception)
                {
                }
            }
                Console.WriteLine(s);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }
    }
}
