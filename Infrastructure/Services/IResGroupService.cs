using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResGroupService: IBaseService<ResGroup>
    {
        Task<PagedResult2<ResGroupBasic>> GetPagedResultAsync(ResGroupPaged val);
        Task<ResGroupDisplay> DefaultGet();
        void Write(IEnumerable<ResGroup> groups, List<ResGroup> implied_ids);
        Task<ResGroup> InsertGroupUserIfNotExist();
        Task AddAllImpliedGroupsToAllUser(IEnumerable<ResGroup> self);
        Task<ResGroup> InsertSettingGroupIfNotExist(string reference, string name);
        Task ResetSecurityData();
        Task AddAllImpliedGroupsToAllUser(IEnumerable<Guid> ids);
        IDictionary<Guid, IList<ResGroup>> _GetTransImplied(IEnumerable<Guid> ids);
        Task<IDictionary<string, ResGroup>> InsertIfNotExist(IDictionary<string, ResGroup> dict);

        Task InsertSecurityData();

        Task UpdateModels();

    }
}
