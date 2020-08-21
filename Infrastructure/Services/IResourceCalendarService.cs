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
    public interface IResourceCalendarService : IBaseService<ResourceCalendar>
    {
        Task<PagedResult2<ResourceCalendarBasic>> GetPaged(ResourceCalendarPaged paged);
        Task<ResourceCalendar> GetDisplayAsync(Guid id);
        Task<ResourceCalendar> CreateResourceCalendar(ResourceCalendarSave val);

        Task UpdateResourceCalendar(Guid id, ResourceCalendarSave val);
    }
}
