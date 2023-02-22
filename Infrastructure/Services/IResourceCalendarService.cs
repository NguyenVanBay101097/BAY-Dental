using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Mapping;
using Umbraco.Web.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResourceCalendarService : IBaseService<ResourceCalendar>
    {
        Task<PagedResult2<ResourceCalendarBasic>> GetPaged(ResourceCalendarPaged paged);
        Task<ResourceCalendarDisplay> GetDisplayAsync(Guid id);
        Task<IEnumerable<AttendanceInterval>> _AttendanceIntervals(Guid id, DateTime start_dt, DateTime end_dt);
        Task<ResourceCalendarNgayGioCongChuan> TinhSoCongChuan(Guid id, DateTime start_dt, DateTime end_dt, IEnumerable<AttendanceInterval> intervals = null);

        Task<ResourceCalendar> CreateResourceCalendar(ResourceCalendarSave val);

        Task UpdateResourceCalendar(Guid id, ResourceCalendarSave val);

        public ResourceCalendarDisplay DefaultGet();
    }
}
