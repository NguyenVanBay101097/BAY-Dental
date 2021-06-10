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

        //Ngày đã gửi: Sent Date
        public DateTime? Date { get; set; }

        //Ngày chờ gửi: Scheduled Date
        /// <summary>
        /// manual, reminder bỏ cột này
        /// </summary>
        public string TypeSend { get; set; }


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
        /// 
        /// waiting : Chờ gửi
        /// sending: Đang gửi
        /// fails: Thất bại
        /// success: Thành công
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Partner, Appointment, SaleOrder, SaleOrderLine
        /// </summary>
        public string ResModel { get; set; }

        //ResCount: Đếm số tin nhắn

        public ICollection<SmsMessagePartnerRel> SmsMessagePartnerRels { get; set; } = new List<SmsMessagePartnerRel>();
        public ICollection<SmsMessageAppointmentRel> SmsMessageAppointmentRels { get; set; } = new List<SmsMessageAppointmentRel>();
        public ICollection<SmsMessageSaleOrderRel> SmsMessageSaleOrderRels { get; set; } = new List<SmsMessageSaleOrderRel>();
        public ICollection<SmsMessageSaleOrderLineRel> SmsMessageSaleOrderLineRels { get; set; } = new List<SmsMessageSaleOrderLineRel>();
    }
}
