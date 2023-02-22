using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareMessagingService : IBaseService<TCareMessaging>
    {
        Task<PagedResult2<TCareMessagingBasic>> GetPagedResultAsync(TCareMessagingPaged val);

        Task<TCareMessagingDisplay> GetDisplay(Guid id);
        Task<TCareMessaging> Create(TCareMessagingSave val);
        Task<TCareMessaging> Update(Guid id, TCareMessagingSave val);
        Task RefeshMessagings(IEnumerable<Guid> ids);


    }
}
