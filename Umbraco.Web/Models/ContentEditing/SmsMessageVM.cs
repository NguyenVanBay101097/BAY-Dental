﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsMessageDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public SmsCampaignBasic SmsCampaign { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DateCreated { get; set; }
        /// <summary>
        /// manual, automatic
        /// </summary>
        public string TypeSend { get; set; }
        public Guid? SmsTemplateId { get; set; }
        public SmsTemplateBasic SmsTemplate { get; set; }
        public string State { get; set; }
        public IEnumerable<PartnerSimpleContact> Partners { get; set; } = new List<PartnerSimpleContact>();
        public int CountPartner { get; set; }
    }

    public class SmsMessageBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DateCreated { get; set; }
        public int CountPartner { get; set; }
    }

    public class SmsMessageSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SmsMessagePaged
    {
        public SmsMessagePaged()
        {
            Limit = 20;
            Offset = 0;
        }
        public int Limit { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? SmsAccountId { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public string State { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class SmsMessageSave
    {
        public SmsMessageSave()
        {
            IsBirthDayManual = false;
            IsAppointmentReminder = false;
        }
        public DateTime? Date { get; set; }
        /// <summary>
        /// manual, reminder
        /// </summary>
        public string TypeSend { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public Guid? SmsTemplateId { get; set; }
        public Guid? SmsAccountId { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public string State { get; set; }
        public IEnumerable<Guid> PartnerIds { get; set; } = new List<Guid>();
        public bool? IsBirthDayManual { get; set; }
        public bool? IsAppointmentReminder { get; set; }
    }
}
