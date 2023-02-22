using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsCareAfterOrderAutomationConfigService : IBaseService<SmsCareAfterOrderAutomationConfig>
    {
        Task<PagedResult2<SmsCareAfterOrderAutomationConfigGrid>> GetPaged(SmsCareAfterOrderAutomationConfigPaged val);
        Task UpdateAsync(Guid id, SmsCareAfterOrderAutomationConfigSave val);
        Task<SmsCareAfterOrderAutomationConfigDisplay> GetDisplay(Guid id);
    }
}
