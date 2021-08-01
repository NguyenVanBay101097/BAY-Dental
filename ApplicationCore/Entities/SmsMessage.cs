using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsMessage : BaseEntity
    {
        public SmsMessage()
        {
            State = "draft";
        }
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        public Guid? SmsCampaignId { get; set; }
        public SmsCampaign SmsCampaign { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public Guid? SmsTemplateId { get; set; }
        public SmsTemplate SmsTemplate { get; set; }
        public string Body { get; set; }
        public Guid? SmsAccountId { get; set; }
        public SmsAccount SmsAccount { get; set; }
        /// <summary>
        /// draft: Mới
        /// in_queue: Chờ gửi
        /// done: Đã gửi
        /// cancelled: Hủy 
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Partner, Appointment, SaleOrder, SaleOrderLine
        /// </summary>
        public string ResModel { get; set; }
        public int? ResCount { get; set; }
        public ICollection<SmsMessagePartnerRel> SmsMessagePartnerRels { get; set; } = new List<SmsMessagePartnerRel>();
        public ICollection<SmsMessageAppointmentRel> SmsMessageAppointmentRels { get; set; } = new List<SmsMessageAppointmentRel>();
        public ICollection<SmsMessageSaleOrderRel> SmsMessageSaleOrderRels { get; set; } = new List<SmsMessageSaleOrderRel>();
        public ICollection<SmsMessageSaleOrderLineRel> SmsMessageSaleOrderLineRels { get; set; } = new List<SmsMessageSaleOrderLineRel>();
    }
}
