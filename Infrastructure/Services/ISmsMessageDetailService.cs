﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsMessageDetailService
    {
        Task<PagedResult2<SmsMessageDetailBasic>> GetPaged(SmsMessageDetailPaged val);
        Task ReSendSms(IEnumerable<SmsMessageDetail> details);
        Task<PagedResult2<SmsMessageDetailStatistic>> GetPagedStatistic(SmsMessageDetailPaged val);
        IQueryable<SmsMessageDetail> SearchQuery();
        Task CreateSmsMessageDetailV2(SmsMessage smsMessage, Guid companyId);
        Task<IEnumerable<ReportTotalOutputItem>> GetReportTotal(ReportTotalInput val);
        Task<PagedResult2<ReportCampaignOutputItem>> GetReportCampaign(ReportCampaignPaged val);
        Task<IEnumerable<ReportSupplierChart>> GetReportSupplierSumary(ReportSupplierPaged val);
    }
}
