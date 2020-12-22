using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ILaboBridgeService: IBaseService<LaboBridge>
    {
        Task<PagedResult2<LaboBridgeBasic>> GetPagedResultAsync(LaboBridgesPaged val);
        Task<LaboBridgeDisplay> GetDisplay(Guid id);
        Task<LaboBridgeDisplay> CreateItem(LaboBridgeSave val);
        Task UpdateItem(Guid id,LaboBridgeSave val);
        Task<IEnumerable<LaboBridgeSimple>> Autocomplete(LaboBridgePageSimple val);

    }
}
