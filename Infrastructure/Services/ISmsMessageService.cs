using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsMessageService
    {
        Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val);
        Task<SmsMessageDisplay> CreateAsync(SmsMessageSave val);
        Task ActionCancel(IEnumerable<Guid> messIds);
        Task UpdateAsync(SmsMessage entity);

        Task<SmsMessage> CreateAsync(SmsMessage entity);
        Task DeleteAsync(SmsMessage entity);
        Task<SmsMessage> GetByIdAsync(Guid id);
        Task SetupSendSmsOrderAutomatic(Guid orderId);
        IQueryable<SmsMessage> SearchQuery();
        Task ActionSend(Guid id);
        Task ActionSendSmsMessageDetail(IEnumerable<SmsMessageDetail> sefts, SmsAccount account);
    }
}
