using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsBirthdayAutomationConfigService : IBaseService<SmsBirthdayAutomationConfig>
    {
        Task UpdateAsync(Guid id, SmsBirthdayAutomationConfigSave val);
        Task<SmsBirthdayAutomationConfigDisplay> GetDisplay(Guid id);
        Task<SmsBirthdayAutomationConfigDisplay> GetByCompany(Guid companyId);
    }
}
