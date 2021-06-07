using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsConfigService : IBaseService<SmsConfig>
    {
        Task<PagedResult2<SmsConfigGrid>> GetPaged(SmsConfigPaged val);
        Task UpdateAsync(Guid id, SmsConfigSave val);
        Task<SmsConfigDisplay> GetDisplay(Guid id);
    }
}
