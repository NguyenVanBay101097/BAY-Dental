﻿using ApplicationCore.Entities;
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
        Task CreateSmsMessageDetail(SmsMessage composer, IEnumerable<Guid> ids, Guid companyId);
        Task SendSMS(IEnumerable<SmsMessageDetail> lines, SmsAccount account);
    }
}
