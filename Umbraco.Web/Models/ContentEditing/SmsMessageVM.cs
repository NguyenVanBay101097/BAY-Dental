using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SmsMessageBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public SmsCampaignBasic SmsCampaign { get; set; }
        public DateTime? Date { get; set; }
        /// <summary>
        /// Send now, reminder
        /// </summary>
        public string TypeSend { get; set; }
        public Guid? SmsTemplateId { get; set; }
        public SmsTemplateBasic SmsTemplate { get; set; }
        public string State { get; set; }
        public IEnumerable<PartnerSimpleContact> Partners { get; set; } = new List<PartnerSimpleContact>();
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
        public int Offset { get; set; }
        public string Search { get; set; }
        public string State { get; set; }

    }

    public class SmsMessageSave
    {
        public Guid? SmsCampaignId { get; set; }
        public DateTime? Date { get; set; }
        /// <summary>
        /// Send now, reminder
        /// </summary>
        public string TypeSend { get; set; }
        public Guid? SmsTemplateId { get; set; }
        public string State { get; set; }
        public IEnumerable<Guid> PartnerIds { get; set; } = new List<Guid>();
    }
}
