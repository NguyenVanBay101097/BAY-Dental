using ApplicationCore.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.Services.SmsSendMessageService;

namespace Infrastructure.Services
{
    public interface ISmsSendMessageService
    {
        Task CreateSmsMessageDetail(CatalogDbContext context, SmsMessage composer, IEnumerable<Guid> ids, Guid companyId);
        Task<Dictionary<Guid, ESMSSendMessageResponseModel>> SendSMS(IEnumerable<SmsMessageDetail> lines, SmsAccount account, CatalogDbContext context);
    }
}
