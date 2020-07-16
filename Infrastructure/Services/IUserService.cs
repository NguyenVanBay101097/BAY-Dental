using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IUserService
    {
        IQueryable<ApplicationUser> GetQueryById(string id);
        void ClearSecurityCache(IEnumerable<string> ids);
        void ClearRuleCache(IEnumerable<string> ids);
        Task<ApplicationUser> GetCurrentUser();
        Task<List<string>> GetGroups();
        Task<bool> HasGroup(string group_ext_id);
        void TestJobFunc(string s, string tenant_id);
        Task UpdateAsync(ApplicationUser user);
        IEnumerable<Guid> GetListCompanyIdsAllowCurrentUser();
    }
}
