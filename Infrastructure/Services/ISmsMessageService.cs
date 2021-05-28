using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsMessageService : IBaseService<SmsMessage>
    {
        Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val);
        Task<SmsMessageDisplay> CreateAsync(SmsMessageSave val);
        Task ActionSendSMS(SmsMessage message);
        Task ActionCancel(IEnumerable<Guid> messIds);
        Task SetupSendSmsOrderAutomatic(Guid orderId);
    }
}
