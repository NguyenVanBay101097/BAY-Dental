using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Mapping;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResourceCalendarAttendanceService : IBaseService<ResourceCalendarAttendance>
    {
        Task<PagedResult2<ResourceCalendarAttendanceBasic>> GetPaged(ResourceCalendarAttendancePaged paged);
        Task<IEnumerable<ResourceCalendarAttendanceDisplay>> GetListResourceCalendadrAtt(ResourceCalendarAttendancePaged val);
        Task SetSequence(IList<ResourceCalendarAttendance> vals);
    }
}
