using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessage : BaseEntity
    {
        public string Name { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }
        public DateTime? Date { get; set; }
        /// <summary>
        /// Send now, reminder
        /// </summary>
        public string TypeSend { get; set; }
        public Guid? SmsTemplateId { get; set; }
        public SmsTemplate SmsTemplate { get; set; }
        public string Body { get; set; }
        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }
        /// <summary>
        /// waiting : Chờ gửi
        /// sending: Đang gửi
        /// fails: Thất bại
        /// success: Thành công
        /// </summary>
        public string State { get; set; }
        public ICollection<SmsMessagePartnerRel> Partners { get; set; } = new List<SmsMessagePartnerRel>();
    }
}
