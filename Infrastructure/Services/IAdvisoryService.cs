using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;


namespace Infrastructure.Services
{
    public interface IAdvisoryService : IBaseService<Advisory>
    {
        Task<PagedResult2<AdvisoryBasic>> GetPagedResultAsync(AdvisoryPaged val);
        Task<AdvisoryDisplay> GetAdvisoryDisplay(Guid id);
        Task<AdvisorySave> CreateAdvisory(AdvisorySave val);
        Task UpdateAdvisory(Guid id, AdvisorySave val);
        Task Unlink(Guid id);
        Task<AdvisoryDisplay> DefaultGet(AdvisoryDefaultGet val);
        Task<ToothAdvised> GetToothAdvise(AdvisoryToothAdvise val);
    }
}
