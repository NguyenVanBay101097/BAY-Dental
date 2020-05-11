using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IHistoryService : IBaseService<History>
    {
        Task<PagedResult2<HistorySimple>> GetPagedResultAsync(HistoryPaged val);
        Task<IEnumerable<HistorySimple>> GetAutocompleteAsync(HistoryPaged val);
        Task<IEnumerable<HistorySimple>> GetResultNotLimitAsync(HistoryPaged val);
        Task<bool> CheckDuplicate(Guid? id,HistorySimple val);
        Task ImportExcelHistories(HistoryImportExcelBaseViewModel val);

    }
}
