using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IDistrictService
    {
        Task<PagedResult<District>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "");
        Task<District> GetDistrictForDisplayAsync(Guid id);
        Task<District> GetDistrictByIdAsync(Guid id);
        Task<District> CreateDistrictAsync(District district);
        Task UpdateDistrictAsync(District district);
        Task UnlinkDistrictAsync(District district);
    }
}
