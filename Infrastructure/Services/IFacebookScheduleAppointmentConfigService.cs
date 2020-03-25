using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookScheduleAppointmentConfigService : IBaseService<FacebookScheduleAppointmentConfig>
    {
        Task<FacebookScheduleAppointmentConfig> CreateFBSheduleConfig(FacebookScheduleAppointmentConfigSave val);
        Task ActionStart(IEnumerable<Guid> ids);
        Task ActionStop(IEnumerable<Guid> ids);
    }
}
