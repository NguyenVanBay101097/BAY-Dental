using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookScheduleAppointmentConfigService : IBaseService<FacebookScheduleAppointmentConfig>
    {
        //Task<PagedResult2<FacebookScheduleAppointmentConfigBasic>> GetPagedResultAsync(FacebookScheduleAppointmentConfigPaged val);
        Task<FacebookScheduleAppointmentConfig> CreateFBSheduleConfig(FacebookScheduleAppointmentConfigSave val);
        Task<FacebookScheduleAppointmentConfig> UpdateFBSheduleConfig(Guid id, FacebookScheduleAppointmentConfigSave val);
        Task ActionStart(IEnumerable<Guid> ids);
        Task ActionStop(IEnumerable<Guid> ids);
    }
}
