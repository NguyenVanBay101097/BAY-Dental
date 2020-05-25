using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareMessagingService : IBaseService<TCareMessaging>
    {
        Task<TCareMessagingDisplay> GetDisplay(Guid id);
        Task<TCareMessaging> Create(TCareMessagingSave val);
        Task<TCareMessaging> Update(Guid id, TCareMessagingSave val);
       
    }
}
