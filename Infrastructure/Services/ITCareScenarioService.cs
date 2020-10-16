using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareScenarioService : IBaseService<TCareScenario>
    {
        Task<TCareScenario> GetDefault();
        Task<PagedResult2<TCareScenarioBasic>> GetPagedResultAsync(TCareScenarioPaged val);
        Task<TCareScenarioDisplay> GetDisplay(Guid id);

        Task<IEnumerable<TCareScenarioBasic>> GetAutocompleteAsync(TCareScenarioPaged val);
    }
}
