using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IWardService
    {
        Task<PagedResult<Ward>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "");
        Task<Ward> GetWardForDisplayAsync(Guid id);
        Task<Ward> GetWardByIdAsync(Guid id);
        Task<Ward> CreateWardAsync(Ward ward);
        Task UpdateWardAsync(Ward ward);
        Task UnlinkWardAsync(Ward ward);
    }
}
