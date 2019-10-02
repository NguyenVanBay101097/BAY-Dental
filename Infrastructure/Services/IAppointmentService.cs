using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAppointmentService: IBaseService<Appointment>
    {
        Task<PagedResult2<AppointmentBasic>> GetPagedResultAsync(AppointmentPaged val);
        Task<AppointmentDisplay> DefaultGet(AppointmentDefaultGet val);
        Task<IEnumerable<AppointmentStateCount>> CountAppointment(DateFromTo dateFromTo);
        Task<AppointmentDisplay> GetAppointmentDisplayAsync(Guid id);
    }
}
