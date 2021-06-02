using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsMessageDetailBasic
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string Number { get; set; }
        public Partner Partner { get; set; }
        public SmsAccountBasic smsAccount { get; set; }
        public string State { get; set; }
        public DateTime? DateCreated { get; set; }
        public string ErrorCode { get; set; }

    }

    public class SmsMessageDetailPaged
    {
        public SmsMessageDetailPaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public Guid? PartnerId { get; set; }
        public string State { get; set; }
        public Guid? SmsMessageId { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

    }

    public class SmsMessageDetailStatistic
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string Number { get; set; }
        public string PartnerName { get; set; }
        public string BrandName { get; set; }
        public string State { get; set; }
        public DateTime? DateCreated { get; set; }
        public string ErrorCode { get; set; }
        public string SmsCampaignName { get; set; }
        public string SmsMessageName { get; set; }
    }

    public class ReportTotalInput
    {
        public DateTime? Date { get; set; }
        public Guid? SmsAccountId { get; set; }
        public Guid? SmsCampaignId { get; set; }
    }

    public class ReportTotalOutputItem
    {
        public string State { get; set; }
        public string StateDisplay { get; set; }
        public int Total { get; set; }
        public float Percentage { get; set; }
    }

    public class ReportCampaignPaged
    {
        public ReportCampaignPaged()
        {
            Limit = 20;
            Offset = 0;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class ReportCampaignOutputItem
    {
        public string SmsCampaignName { get; set; }
        public int TotalMessages { get; set; }
        public int TotalSuccessfulMessages { get; set; }
        public int TotalFailedMessages { get; set; }
    }

    public class ReportSupplierInput
    {
        public string SmsSupplierCode { get; set; }
    }

    public class ReportSupplierPaged
    {
        public Guid AccountId { get; set; }
        public string State { get; set; }
    }

    public class ReportSupplierOutputItem
    {
        public string State { get; set; }
        public string StateDisplay { get; set; }
        public IEnumerable<ReportSupplierOutputItemData> Data { get; set; } = new List<ReportSupplierOutputItemData>();
    }

    public class ReportSupplierChart
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
    }

    public class ReportSupplierOutputItemData
    {
        public DateTime Date { get; set; }
        public int Total { get; set; }
    }
}

