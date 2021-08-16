﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICustomerReceiptReportService 
    {
        Task<PagedResult2<CustomerReceiptReportBasic>> GetPagedResultAsync(CustomerReceiptReportFilter val);

        Task<PagedResult2<CustomerReceiptReportTime>> GetCustomerReceiptForTime(CustomerReceiptReportFilter val);

        Task<PagedResult2<CustomerReceiptReportBasic>> GetCustomerReceiptForTimeDetail(CustomerReceiptTimeDetailFilter val);

        Task<IEnumerable<CustomerReceiptGetCountItem>> GetCountCustomerReceipt(CustomerReceiptReportFilter val);

        Task<IEnumerable<CustomerReceiptGetCountItem>> GetCountCustomerReceiptNotreatment(CustomerReceiptReportFilter val);

        Task<long> GetCountTime(CustomerReceiptReportFilter val);
        Task<CustomerReceiptReportPdf<CustomerReceiptReportBasic>> GetPagedResultPdf(CustomerReceiptReportFilter val);
        Task<CustomerReceiptReportPdf<CustomerReceiptForTimePdf>> GetCustomerReceiptForTimePdf(CustomerReceiptReportFilter val);
    }
}
