using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IFacebookMassMessagingService: IBaseService<FacebookMassMessaging>
    {
        Task<PagedResult2<FacebookMassMessagingBasic>> GetPagedResultAsync(FacebookMassMessagingPaged val);
        Task ActionSend(IEnumerable<Guid> ids);
        Task SetScheduleDate(FacebookMassMessagingSetScheduleDate val);
        Task ActionCancel(IEnumerable<Guid> ids);
        Task Unlink(IEnumerable<Guid> ids);
        Task<PagedResult2<FacebookUserProfileBasic>> ActionViewDelivered(Guid id, FacebookMassMessagingStatisticsPaged paged);
    }
}
